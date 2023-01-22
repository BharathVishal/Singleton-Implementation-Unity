#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation.Platform
{
    class IosRewardedAd : IosNativeObject, IPlatformRewardedAd
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnShowed;
        public event EventHandler OnClicked;
        public event EventHandler OnClosed;
        public event EventHandler<ShowErrorEventArgs> OnFailedShow;
        public event EventHandler<RewardEventArgs> OnUserRewarded;

        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdState")) return AdState.Unloaded;
                return (AdState)RewardedAdGetAdState(NativePtr);
            }
        }
        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdUnitId")) return null;
                return RewardedAdGetAdUnitId(NativePtr);
            }
        }

        IosRewardedLoadListener m_RewardedLoadListener;
        IosRewardedShowListener m_RewardedShowListener;

        public IosRewardedAd(string adUnitId) : base(true)
        {
            NativePtr = RewardedAdCreate(adUnitId);
        }

        public void Load()
        {
            if (CheckDisposedAndLogError("Cannot call Load()")) return;
            if (m_RewardedLoadListener == null)
            {
                m_RewardedLoadListener = new IosRewardedLoadListener();
            }
            RewardedAdLoad(NativePtr, m_RewardedLoadListener.NativePtr);
        }

        public void Show(RewardedAdShowOptions showOptions = null)
        {
            if (CheckDisposedAndLogError("Cannot call Show()")) return;
            if (m_RewardedShowListener == null)
            {
                m_RewardedShowListener = new IosRewardedShowListener();
            }

            if (showOptions == null)
            {
                showOptions = new RewardedAdShowOptions();
            }

            RewardedAdShow(NativePtr, m_RewardedShowListener.NativePtr, showOptions.S2SData);
        }

        public override void Dispose()
        {
            m_RewardedLoadListener?.Dispose();
            m_RewardedLoadListener = null;
            m_RewardedShowListener?.Dispose();
            m_RewardedShowListener = null;
            base.Dispose();
        }

        ~IosRewardedAd()
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

        internal void InvokeShownEvent()
        {
            ThreadUtil.Post(state => OnShowed?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeUserRewardedEvent(RewardEventArgs args)
        {
            ThreadUtil.Post(state => OnUserRewarded?.Invoke(this, args));
        }

        internal void InvokeClickedEvent()
        {
            ThreadUtil.Post(state => OnClicked?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeClosedEvent()
        {
            ThreadUtil.Post(state => OnClosed?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeFailedShowEvent(ShowErrorEventArgs args)
        {
            ThreadUtil.Post(state => OnFailedShow?.Invoke(this, args));
        }

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdCreate")]
        static extern IntPtr RewardedAdCreate(string adUnitId);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdLoad")]
        static extern void RewardedAdLoad(IntPtr nativePtr, IntPtr listener);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdShow")]
        static extern void RewardedAdShow(IntPtr nativePtr, IntPtr listener, S2SRedeemData showOptions);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdGetAdUnitId")]
        static extern string RewardedAdGetAdUnitId(IntPtr nativePtr);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdGetAdState")]
        static extern int RewardedAdGetAdState(IntPtr nativePtr);
    }
}
#endif
