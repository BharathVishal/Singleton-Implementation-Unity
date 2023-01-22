using System;
using UnityEngine;
using Unity.Services.Core;

namespace Unity.Services.Mediation.Samples
{
    public class BannerExample : MonoBehaviour
    {
        [Header("Ad Unit Ids"), Tooltip("Android Ad Unit Ids")]
        public string androidAdUnitId;
        [Tooltip("iOS Ad Unit Ids")]
        public string iosAdUnitId;

        [Header("Game Ids"), Tooltip("[Optional] Specifies the iOS GameId. Otherwise uses the dashboard provided GameId by default.")]
        public string iosGameId;
        [Tooltip("[Optional] Specifies the Android GameId. Otherwise uses the dashboard provided GameId by default.")]
        public string androidGameId;

        [Header("Banner options")]
        public BannerAdAnchor bannerAdAnchor = BannerAdAnchor.TopCenter;

        public BannerAdPredefinedSize bannerSize = BannerAdPredefinedSize.Banner;

        IBannerAd m_BannerAd;

        async void Start()
        {
            try
            {
                Debug.Log("Initializing...");
                await UnityServices.InitializeAsync(GetGameId());
                Debug.Log("Initialized!");

                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        void OnDestroy()
        {
            m_BannerAd.Dispose();
        }

        InitializationOptions GetGameId()
        {
            var initializationOptions = new InitializationOptions();

#if UNITY_IOS
                if (!string.IsNullOrEmpty(iosGameId))
                {
                    initializationOptions.SetGameId(iosGameId);
                }
#elif UNITY_ANDROID
                if (!string.IsNullOrEmpty(androidGameId))
                {
                    initializationOptions.SetGameId(androidGameId);
                }
#endif

            return initializationOptions;
        }

        void InitializationComplete()
        {
            // Impression Event
            MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;

            var bannerAdSize = bannerSize.ToBannerAdSize();
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    m_BannerAd = MediationService.Instance.CreateBannerAd(androidAdUnitId, bannerAdSize, bannerAdAnchor);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    m_BannerAd = MediationService.Instance.CreateBannerAd(iosAdUnitId, bannerAdSize, bannerAdAnchor);
                    break;
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxEditor:
                    m_BannerAd = MediationService.Instance.CreateBannerAd(!string.IsNullOrEmpty(androidAdUnitId) ? androidAdUnitId : iosAdUnitId, bannerAdSize, bannerAdAnchor);
                    break;
                default:
                    Debug.LogWarning("Mediation service is not available for this platform:" + Application.platform);
                    return;
            }

            Debug.Log("Initialized On Start! Loading banner Ad...");
            LoadAd();
        }

        async void LoadAd()
        {
            try
            {
                await m_BannerAd.LoadAsync();
                AdLoaded();
            }
            catch (LoadFailedException e)
            {
                AdFailedLoad(e);
            }
        }

        void AdLoaded()
        {
            Debug.Log("Ad loaded");
        }

        void AdFailedLoad(LoadFailedException e)
        {
            Debug.Log("Failed to load ad");
            Debug.Log(e.Message);
        }

        void ImpressionEvent(object sender, ImpressionEventArgs args)
        {
            var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
            Debug.Log($"Impression event from ad unit id {args.AdUnitId} : {impressionData}");
        }

        void InitializationFailed(Exception error)
        {
            var initializationError = SdkInitializationError.Unknown;
            if (error is InitializeFailedException initializeFailedException)
            {
                initializationError = initializeFailedException.initializationError;
            }

            Debug.Log($"Initialization Failed: {initializationError}:{error.Message}");
        }
    }
}
