using System;
using System.Threading.Tasks;
using Unity.Services.Mediation.Platform;
using UnityEngine;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Class to be instantiated in order to show a Banner Ad.
    /// </summary>
    public sealed class BannerAd : IBannerAd
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
        /// Event to be triggered by the adapter when the user clicks on the Ad.
        /// </summary>
        public event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad refreshes
        /// </summary>
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;

        /// <summary>
        /// Get the current state of the ad.
        /// </summary>
        public AdState AdState => m_BannerAdImpl.AdState;

        /// <summary>
        /// Get the ad unit id set during construction.
        /// </summary>
        public string AdUnitId => m_BannerAdImpl.AdUnitId;

        /// <summary>
        /// Get the banner size set during construction.
        /// </summary>
        public BannerAdSize Size => m_BannerAdImpl.Size;

        IPlatformBannerAd m_BannerAdImpl;
        TaskCompletionSource<object> m_LoadCompletionSource;
        bool m_IsLoading;

        /// <summary>
        /// Constructor for managing a specific Banner Ad.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        /// <param name="size">Size of banner set to be constructed.</param>
        /// <param name="anchor">Anchor on which the banner position is based</param>
        /// <param name="positionOffset">The X, Y coordinates offsets, relative to the anchor point</param>
        public BannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2())
        {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            m_BannerAdImpl = new EditorBannerAd(adUnitId, size, anchor, positionOffset);
#elif UNITY_ANDROID
            m_BannerAdImpl = new AndroidBannerAd(adUnitId, size, anchor, positionOffset);
#elif UNITY_IOS
            m_BannerAdImpl = new IosBannerAd(adUnitId, size, anchor, positionOffset);
#else
            m_BannerAdImpl = new UnsupportedBannerAd(adUnitId, size, anchor, positionOffset);
#endif
            InitializeCallbacks();
        }

        /// <summary>
        /// Sets the position of a banner ad.
        /// </summary>
        /// <param name="anchor">Anchor on which the banner position is based</param>
        /// <param name="positionOffset">The X, Y coordinates offsets, relative to the anchor point</param>
        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2()) => m_BannerAdImpl.SetPosition(anchor, positionOffset);

        internal BannerAd(IPlatformBannerAd bannerAdImpl)
        {
            m_BannerAdImpl = bannerAdImpl;
            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            m_BannerAdImpl.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_BannerAdImpl.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_BannerAdImpl.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_BannerAdImpl.OnRefreshed += (sender, args) => OnRefreshed?.Invoke(this, args);
        }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        /// <returns>Async Load task</returns>
        /// <exception cref="Unity.Services.Mediation.LoadFailedException">Thrown when the ad failed to load</exception>
        public Task LoadAsync()
        {
            if (!m_IsLoading)
            {
                SetupAsyncLoad();
                m_BannerAdImpl.Load();
            }

            return m_LoadCompletionSource?.Task ?? Task.CompletedTask;
        }

        void SetupAsyncLoad()
        {
            m_LoadCompletionSource = new TaskCompletionSource<object>();
            m_BannerAdImpl.OnLoaded += OnLoadCompleted;
            m_BannerAdImpl.OnFailedLoad += OnLoadFailed;
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
            m_BannerAdImpl.OnFailedLoad -= OnLoadFailed;
            m_BannerAdImpl.OnLoaded -= OnLoadCompleted;
            m_IsLoading = false;
        }

        /// <summary>
        /// Dispose and release internal resources.
        /// </summary>
        public void Dispose() => m_BannerAdImpl.Dispose();
    }
}
