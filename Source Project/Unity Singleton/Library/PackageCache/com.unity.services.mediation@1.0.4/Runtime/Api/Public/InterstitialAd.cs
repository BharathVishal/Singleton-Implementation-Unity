using System;
using System.Threading.Tasks;
using Unity.Services.Mediation.Platform;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Class to be instantiated in order to show an Interstitial Ad.
    /// </summary>
    public sealed class InterstitialAd : IInterstitialAd
    {
        /// <summary>
        /// Event to be triggered by the adapter when an Ad is loaded.
        /// </summary>
        public event EventHandler OnLoaded;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad fails to load.
        /// </summary>
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad is started.
        /// </summary>
        public event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the Ad.
        /// </summary>
        public event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad is closed.
        /// </summary>
        public event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad has an error.
        /// </summary>
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Get the current state of the ad.
        /// </summary>
        public AdState AdState => m_InterstitialAdImpl.AdState;

        /// <summary>
        /// Get the ad unit id set during construction.
        /// </summary>
        public string AdUnitId => m_InterstitialAdImpl.AdUnitId;

        IPlatformInterstitialAd m_InterstitialAdImpl;
        TaskCompletionSource<object> m_LoadCompletionSource;
        TaskCompletionSource<object> m_ShowCompletionSource;
        bool m_IsLoading;
        bool m_IsShowing;

        /// <summary>
        /// Constructor for managing a specific Interstitial Ad.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        public InterstitialAd(string adUnitId)
        {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            m_InterstitialAdImpl = new EditorInterstitialAd(adUnitId);
#elif UNITY_ANDROID
            m_InterstitialAdImpl = new AndroidInterstitialAd(adUnitId);
#elif UNITY_IOS
            m_InterstitialAdImpl = new IosInterstitialAd(adUnitId);
#else
            m_InterstitialAdImpl = new UnsupportedInterstitialAd(adUnitId);
#endif
            InitializeCallbacks();
        }

        internal InterstitialAd(IPlatformInterstitialAd interstitialAdImpl)
        {
            m_InterstitialAdImpl = interstitialAdImpl;
            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            m_InterstitialAdImpl.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_InterstitialAdImpl.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_InterstitialAdImpl.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_InterstitialAdImpl.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_InterstitialAdImpl.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_InterstitialAdImpl.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
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
                m_InterstitialAdImpl.Load();
            }

            return m_LoadCompletionSource?.Task ?? Task.CompletedTask;
        }

        void SetupAsyncLoad()
        {
            m_LoadCompletionSource = new TaskCompletionSource<object>();
            m_InterstitialAdImpl.OnLoaded += OnLoadCompleted;
            m_InterstitialAdImpl.OnFailedLoad += OnLoadFailed;
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
            m_InterstitialAdImpl.OnFailedLoad -= OnLoadFailed;
            m_InterstitialAdImpl.OnLoaded -= OnLoadCompleted;
            m_IsLoading = false;
        }

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        /// <param name="showOptions">Optional, allows setting optional parameters for showing an interstitial ad.</param>
        /// <returns>ShowAsync Task</returns>
        /// <exception cref="Unity.Services.Mediation.ShowFailedException">Thrown when the ad failed to show</exception>
        public Task ShowAsync(InterstitialAdShowOptions showOptions = null)
        {
            if (!m_IsShowing)
            {
                SetupAsyncShow();

                if (showOptions != null && showOptions.AutoReload)
                {
                    m_InterstitialAdImpl.OnFailedShow += ReloadAd;
                    m_InterstitialAdImpl.OnClosed += ReloadAd;
                }

                m_InterstitialAdImpl.Show();
            }

            return m_ShowCompletionSource?.Task ?? Task.CompletedTask;
        }

        void SetupAsyncShow()
        {
            m_ShowCompletionSource = new TaskCompletionSource<object>();
            m_InterstitialAdImpl.OnClosed += OnShowCompleted;
            m_InterstitialAdImpl.OnFailedShow += OnShowFailed;
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
            m_InterstitialAdImpl.OnFailedShow -= OnShowFailed;
            m_InterstitialAdImpl.OnClosed -= OnShowCompleted;
            m_IsShowing = false;
        }

        void ReloadAd(object sender, EventArgs e)
        {
            m_InterstitialAdImpl.OnFailedShow -= ReloadAd;
            m_InterstitialAdImpl.OnClosed -= ReloadAd;
            try
            {
                LoadAsync();
            }
            catch (LoadFailedException) {}
        }

        /// <summary>
        /// Dispose and release internal resources.
        /// </summary>
        public void Dispose() => m_InterstitialAdImpl.Dispose();
    }
}
