using System;
using System.Threading.Tasks;
using Unity.Services.Analytics.Data;
using Unity.Services.Analytics.Internal;
using Unity.Services.Analytics.Platform;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using UnityEngine;
using Event = Unity.Services.Analytics.Internal.Event;

namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance : IAnalyticsService
    {
        public string PrivacyUrl => "https://unity3d.com/legal/privacy-policy";

        const string k_CollectUrlPattern = "https://collect.analytics.unity3d.com/api/analytics/collect/v1/projects/{0}/environments/{1}";
        const string k_ForgetCallingId = "com.unity.services.analytics.Events." + nameof(OptOut);

        internal IPlayerId PlayerId { get; private set; }
        internal IInstallationId InstallId { get; private set; }
        internal ICloudProjectId CloudProjectIdProvider { get; private set; }
        internal string CloudProjectId => CloudProjectIdProvider?.GetCloudProjectId() ?? Application.cloudProjectId;

        internal string CustomAnalyticsId { get; private set; }

        internal IBuffer dataBuffer = new Internal.Buffer();
        int m_BufferLengthAtLastGameRunning;

        internal IDataGenerator dataGenerator = new DataGenerator();

        internal IDispatcher dataDispatcher { get; set; }

        string m_CollectURL;
        readonly StdCommonParams m_CommonParams = new StdCommonParams();
        readonly string m_StartUpCallingId = "com.unity.services.analytics.Events.Startup";

        internal IIDeviceIdentifiersInternal deviceIdentifiersInternal = new DeviceIdentifiersInternal();

        internal bool ServiceEnabled { get; private set; } = true;

        internal ICoreStatsHelper m_CoreStatsHelper = new CoreStatsHelper();
        internal IConsentTracker ConsentTracker;

        public string SessionID { get; }

        internal AnalyticsServiceInstance()
        {
            ConsentTracker = new ConsentTracker(m_CoreStatsHelper);

            // Add a check to ensure a project id is set.
            if (string.IsNullOrEmpty(Application.cloudProjectId))
            {
                Debug.LogError("No cloud project ID was found by the Analytics SDK. This means Analytics events will not be sent. Please make sure to link your cloud project in the Unity editor to fix this problem.");
                return;
            }

            dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper(), ConsentTracker);

            SessionID = Guid.NewGuid().ToString();

            m_CommonParams.ClientVersion = Application.version;
            m_CommonParams.ProjectID = Application.cloudProjectId;
            m_CommonParams.GameBundleID = Application.identifier;
            m_CommonParams.Platform = Runtime.Name();
            m_CommonParams.BuildGuuid = Application.buildGUID;
            m_CommonParams.Idfv = deviceIdentifiersInternal.Idfv;
        }

        public void Flush()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (string.IsNullOrEmpty(CloudProjectId))
            {
                return;
            }

            if (InstallId == null)
            {
#if UNITY_ANALYTICS_DEVELOPMENT
                Debug.Log("The Core callback hasn't yet triggered.");
#endif

                return;
            }

            if (ConsentTracker.IsGeoIpChecked() && ConsentTracker.IsConsentGiven())
            {
                dataBuffer.InstallID = InstallId.GetOrCreateIdentifier();
                dataBuffer.PlayerID = PlayerId?.PlayerId;

                dataBuffer.UserID = GetAnalyticsUserID();

                dataBuffer.SessionID = SessionID;
                dataDispatcher.CollectUrl = m_CollectURL;
                dataDispatcher.Flush();
            }

            if (ConsentTracker.IsOptingOutInProgress())
            {
                analyticsForgetter.AttemptToForget();
            }
        }

        public void RecordInternalEvent(Event eventToRecord)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataBuffer.PushEvent(eventToRecord);
        }

        internal void SetDependencies(ICloudProjectId cloudProjectId, IInstallationId installId, IPlayerId playerId, string environment, string customAnalyticsId)
        {
            CloudProjectIdProvider = cloudProjectId;
            InstallId = installId;
            PlayerId = playerId;
            CustomAnalyticsId = customAnalyticsId;

            m_CommonParams.ProjectID = CloudProjectId;
            m_CollectURL = string.Format(k_CollectUrlPattern, CloudProjectId, environment.ToLowerInvariant());
        }

        internal async Task Initialize(ICloudProjectId cloudProjectId, IInstallationId installId, IPlayerId playerId, string environment, string customAnalyticsId)
        {
            SetDependencies(cloudProjectId, installId, playerId, environment, customAnalyticsId);

            if (!ServiceEnabled)
            {
                return;
            }

            await InitializeUser();
        }

        async Task InitializeUser()
        {
            SetVariableCommonParams();

#if UNITY_ANALYTICS_DEVELOPMENT
            Debug.LogFormat("UA2 Setup\nCollectURL:{0}\nSessionID:{1}", m_CollectURL, SessionID);
#endif

            try
            {
                await ConsentTracker.CheckGeoIP();

                if (ConsentTracker.IsGeoIpChecked() && (ConsentTracker.IsConsentDenied() || ConsentTracker.IsOptingOutInProgress()))
                {
                    OptOut();
                }
            }
#if UNITY_ANALYTICS_EVENT_LOGS
            catch (ConsentCheckException e)
            {
                Debug.Log("Initial GeoIP lookup fail: " + e.Message);
            }
#else
            catch (ConsentCheckException)
            {
            }
#endif
        }

        /// <summary>
        /// Sets up the internals of the Analytics SDK, including the regular sending of events and assigning
        /// the userID to be used in further event recording.
        /// </summary>
        internal void Startup()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            // Startup Events.
            dataGenerator.SdkStartup(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId);
            dataGenerator.ClientDevice(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, SystemInfo.processorType, SystemInfo.graphicsDeviceName, SystemInfo.processorCount, SystemInfo.systemMemorySize, Screen.width, Screen.height, (int)Screen.dpi);

#if UNITY_DOTSRUNTIME
            var isTiny = true;
#else
            var isTiny = false;
#endif

            dataGenerator.GameStarted(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, Application.buildGUID, SystemInfo.operatingSystem, isTiny, DebugDevice.IsDebugDevice(), Locale.AnalyticsRegionLanguageCode());
        }

        internal void NewPlayerEvent()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            if (InstallId != null && new InternalNewPlayerHelper(InstallId).IsNewPlayer())
            {
                dataGenerator.NewPlayer(ref dataBuffer, DateTime.Now, m_CommonParams, m_StartUpCallingId, SystemInfo.deviceModel);
            }
        }

        /// <summary>
        /// Records the gameEnded event, and flushes the buffer to upload all events.
        /// </summary>
        internal void GameEnded()
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataGenerator.GameEnded(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.Events.Shutdown", DataGenerator.SessionEndState.QUIT);

            // Need to null check the consent tracker here in case the game ends before the tracker can be initialised.
            if (ConsentTracker != null && ConsentTracker.IsGeoIpChecked())
            {
                Flush();
            }
        }

        internal void RecordGameRunningIfNecessary()
        {
            if (ServiceEnabled)
            {
                if (dataBuffer.Length == 0 || dataBuffer.Length == m_BufferLengthAtLastGameRunning)
                {
                    SetVariableCommonParams();
                    dataGenerator.GameRunning(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.AnalyticsServiceInstance.RecordGameRunningIfNecessary");
                    m_BufferLengthAtLastGameRunning = dataBuffer.Length;
                }
                else
                {
                    m_BufferLengthAtLastGameRunning = dataBuffer.Length;
                }
            }
        }

        // <summary>
        // Internal tick is called by the Heartbeat at set intervals.
        // </summary>
        internal void InternalTick()
        {
            if (ServiceEnabled &&
                ConsentTracker.IsGeoIpChecked())
            {
                Flush();
            }
        }

        void SetVariableCommonParams()
        {
            m_CommonParams.Idfv = deviceIdentifiersInternal.Idfv;
            m_CommonParams.DeviceVolume = DeviceVolumeProvider.GetDeviceVolume();
            m_CommonParams.BatteryLoad = SystemInfo.batteryLevel;
            m_CommonParams.UasUserID = PlayerId?.PlayerId;
        }

        void GameEnded(DataGenerator.SessionEndState quitState)
        {
            if (!ServiceEnabled)
            {
                return;
            }

            dataGenerator.GameEnded(ref dataBuffer, DateTime.Now, m_CommonParams, "com.unity.services.analytics.Events.GameEnded", quitState);
        }

        public async Task SetAnalyticsEnabled(bool enabled)
        {
            if (enabled && !ServiceEnabled)
            {
                dataBuffer = new Internal.Buffer();
                dataDispatcher = new Dispatcher(dataBuffer, new WebRequestHelper(), ConsentTracker);
                await InitializeUser();

                ServiceEnabled = true;
            }
            else if (!enabled && ServiceEnabled)
            {
                dataBuffer.ClearBuffer();
                dataBuffer.ClearDiskCache();
                dataBuffer = new BufferRevoked();

                ServiceEnabled = false;
            }
        }
    }
}
