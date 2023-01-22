using System;
using System.Threading.Tasks;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Scheduler.Internal;
using Unity.Services.Mediation.Platform;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Class to be instantiated in order to show a Rewarded Ad.
    /// </summary>
    public sealed class RewardedAd : IRewardedAd
    {
        internal const double k_ReloadDelay = 1.0;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad is loaded.
        /// </summary>
        public event EventHandler OnLoaded;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad fails to load.
        /// </summary>
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        /// <summary>
        /// Event to be triggered by the adapter when a Rewarded Ad is shown.
        /// </summary>
        public event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the RewardedAd.
        /// </summary>
        public event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd is closed.
        /// </summary>
        public event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd has an error.
        /// </summary>
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Event to be triggered by the adapter when a reward needs to be issued.
        /// </summary>
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        /// <summary>
        ///<value>Gets the state of the <c>RewardedAd</c>.</value>
        /// </summary>
        public AdState AdState => m_RewardedAdImpl.AdState;

        /// <summary>
        /// <value>Gets the id of the ad unit.</value>
        /// </summary>
        public string AdUnitId => m_RewardedAdImpl.AdUnitId;

        IPlatformRewardedAd m_RewardedAdImpl;
        TaskCompletionSource<object> m_LoadCompletionSource;
        TaskCompletionSource<object> m_ShowCompletionSource;
        bool m_IsLoading;
        bool m_IsShowing;

        /// <summary>
        /// Constructor for managing a specific Rewarded Ad.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        public RewardedAd(string adUnitId)
        {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            m_RewardedAdImpl = new EditorRewardedAd(adUnitId);
#elif UNITY_ANDROID
            m_RewardedAdImpl = new AndroidRewardedAd(adUnitId);
#elif UNITY_IOS
            m_RewardedAdImpl = new IosRewardedAd(adUnitId);
#else
            m_RewardedAdImpl = new UnsupportedRewardedAd(adUnitId);
#endif
            InitializeCallbacks();
        }

        internal RewardedAd(IPlatformRewardedAd rewardedAdImpl)
        {
            m_RewardedAdImpl = rewardedAdImpl;
            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            m_RewardedAdImpl.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_RewardedAdImpl.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_RewardedAdImpl.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_RewardedAdImpl.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_RewardedAdImpl.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_RewardedAdImpl.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
            m_RewardedAdImpl.OnUserRewarded += (sender, args) => OnUserRewarded?.Invoke(this, args);
        }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        /// <returns>LoadAsync Task</returns>
        /// <exception cref="Unity.Services.Mediation.LoadFailedException">Thrown when the ad failed to load</exception>
        public Task LoadAsync()
        {
            if (!m_IsLoading)
            {
                SetupAsyncLoad();
                m_RewardedAdImpl.Load();
            }

            return m_LoadCompletionSource?.Task ?? Task.CompletedTask;
        }

        void SetupAsyncLoad()
        {
            m_LoadCompletionSource = new TaskCompletionSource<object>();
            m_RewardedAdImpl.OnLoaded += OnLoadCompleted;
            m_RewardedAdImpl.OnFailedLoad += OnLoadFailed;
            m_IsLoading = true;
        }

        void OnLoadCompleted(object sender, EventArgs e)
        {
            m_LoadCompletionSource.TrySetResult(null);
            TearDownAsyncLoad();
        }

        void OnLoadFailed(object sender, LoadErrorEventArgs args)
        {
            m_LoadCompletionSource.SetException(new LoadFailedException(args.Error, args.Message));
            TearDownAsyncLoad();
        }

        void TearDownAsyncLoad()
        {
            m_RewardedAdImpl.OnFailedLoad -= OnLoadFailed;
            m_RewardedAdImpl.OnLoaded -= OnLoadCompleted;
            m_IsLoading = false;
        }

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        /// <param name="showOptions">Optional, allows setting optional parameters for showing a rewarded ad.</param>
        /// <returns>ShowAsync Task</returns>
        /// <exception cref="Unity.Services.Mediation.ShowFailedException">Thrown when the ad failed to show</exception>
        public Task ShowAsync(RewardedAdShowOptions showOptions = null)
        {
            if (!m_IsShowing)
            {
                SetupAsyncShow();

                if (showOptions != null && showOptions.AutoReload)
                {
                    m_RewardedAdImpl.OnFailedShow += ReloadAd;
                    m_RewardedAdImpl.OnClosed += ReloadAd;
                }

                m_RewardedAdImpl.Show(showOptions);
            }

            return m_ShowCompletionSource?.Task ?? Task.CompletedTask;
        }

        void SetupAsyncShow()
        {
            m_ShowCompletionSource = new TaskCompletionSource<object>();
            m_RewardedAdImpl.OnClosed += OnShowCompleted;
            m_RewardedAdImpl.OnFailedShow += OnShowFailed;
            m_IsShowing = true;
        }

        void OnShowCompleted(object sender, EventArgs e)
        {
            m_ShowCompletionSource.TrySetResult(null);
            TearDownAsyncShow();
        }

        void OnShowFailed(object sender, ShowErrorEventArgs args)
        {
            m_ShowCompletionSource.TrySetException(new ShowFailedException(args.Error, args.Message));
            TearDownAsyncShow();
        }

        void TearDownAsyncShow()
        {
            m_RewardedAdImpl.OnFailedShow -= OnShowFailed;
            m_RewardedAdImpl.OnClosed -= OnShowCompleted;
            m_IsShowing = false;
        }

        void ReloadAd(object sender, EventArgs e)
        {
            m_RewardedAdImpl.OnFailedShow -= ReloadAd;
            m_RewardedAdImpl.OnClosed -= ReloadAd;

            IActionScheduler actionScheduler = CoreRegistry.Instance.GetServiceComponent<IActionScheduler>();
            actionScheduler.ScheduleAction(() =>
            {
                try
                {
                    LoadAsync();
                }
                catch (LoadFailedException) {}
            }, k_ReloadDelay);
        }

        /// <summary>
        /// Dispose and release internal resources.
        /// </summary>
        public void Dispose() => m_RewardedAdImpl.Dispose();
    }
}
