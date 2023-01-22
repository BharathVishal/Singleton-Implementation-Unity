#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation.Platform
{
    class IosInterstitialAd : IosNativeObject, IPlatformInterstitialAd
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdState")) return AdState.Unloaded;
                return (AdState)InterstitialAdGetAdState(NativePtr);
            }
        }

        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdUnitId")) return null;
                return InterstitialAdGetAdUnitId(NativePtr);
            }
        }

        IosInterstitialLoadListener m_InterstitialLoadListener;
        IosInterstitialShowListener m_InterstitialShowListener;

        public IosInterstitialAd(string adUnitId) : base(true)
        {
            NativePtr = InterstitialAdCreate(adUnitId);
        }

        public void Load()
        {
            if (CheckDisposedAndLogError("Cannot call Load()")) return;
            if (m_InterstitialLoadListener == null)
            {
                m_InterstitialLoadListener = new IosInterstitialLoadListener();
            }
            InterstitialAdLoad(NativePtr, m_InterstitialLoadListener.NativePtr);
        }

        public void Show()
        {
            if (CheckDisposedAndLogError("Cannot call Show()")) return;
            if (m_InterstitialShowListener == null)
            {
                m_InterstitialShowListener = new IosInterstitialShowListener();
            }
            InterstitialAdShow(NativePtr, m_InterstitialShowListener.NativePtr);
        }

        public override void Dispose()
        {
            m_InterstitialLoadListener?.Dispose();
            m_InterstitialLoadListener = null;
            m_InterstitialShowListener?.Dispose();
            m_InterstitialShowListener = null;
            base.Dispose();
        }

        ~IosInterstitialAd()
        {
            Dispose();
        }

        internal void InvokeLoadedEvent()
        {
            ThreadUtil.Post(state => OnLoaded?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeFailedLoadEvent(LoadErrorEventArgs args)
        {
            ThreadUtil.Post(state => OnFailedLoad?.Invoke(this, args));
        }

        internal void InvokeStartedEvent()
        {
            ThreadUtil.Post(state => OnShowed?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeClickedEvent()
        {
            ThreadUtil.Post(state => OnClicked?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeFinishedEvent()
        {
            ThreadUtil.Post(state => OnClosed?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeFailedShowEvent(ShowErrorEventArgs args)
        {
            ThreadUtil.Post(state => OnFailedShow?.Invoke(this, args));
        }

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdCreate")]
        static extern IntPtr InterstitialAdCreate(string adUnitId);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdLoad")]
        static extern void InterstitialAdLoad(IntPtr nativePtr, IntPtr listener);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdShow")]
        static extern void InterstitialAdShow(IntPtr nativePtr, IntPtr listener);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdGetAdUnitId")]
        static extern string InterstitialAdGetAdUnitId(IntPtr nativePtr);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdGetAdState")]
        static extern int InterstitialAdGetAdState(IntPtr nativePtr);
    }
}
#endif
