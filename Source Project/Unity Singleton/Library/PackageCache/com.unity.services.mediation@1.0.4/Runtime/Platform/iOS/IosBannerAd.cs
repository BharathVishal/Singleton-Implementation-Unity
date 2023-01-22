#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class IosBannerAd : IosNativeObject, IPlatformBannerAd
    {
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnClicked;
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;

        public AdState AdState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdState")) return AdState.Unloaded;
                return (AdState)BannerAdGetAdState(NativePtr);
            }
        }
        public string AdUnitId
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call AdUnitId")) return null;
                return BannerAdGetAdUnitId(NativePtr);
            }
        }

        public BannerAdSize Size { get; }
        IosBannerLoadListener m_BannerLoadListener;

        public IosBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2()) : base(true)
        {
            NativePtr = BannerAdCreate(adUnitId, size.DpWidth, size.DpHeight);
            Size = size;
            SetPosition(anchor, positionOffset);
        }

        public void Load()
        {
            if (CheckDisposedAndLogError("Cannot call Load()")) return;
            if (m_BannerLoadListener == null)
            {
                m_BannerLoadListener = new IosBannerLoadListener();
            }
            BannerAdLoad(NativePtr, m_BannerLoadListener.NativePtr);
        }

        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2())
        {
            if (CheckDisposedAndLogError("Cannot set Banner Position")) return;

            // Using the dp ratio won't match the exact position, therefore we pass a screen ratio
            positionOffset.x /= Screen.width;
            positionOffset.y /= Screen.height;

            BannerAdSetPosition(NativePtr, (int)anchor, positionOffset.x, positionOffset.y);
        }

        public override void Dispose()
        {
            if (NativePtr != IntPtr.Zero)
            {
                BannerAdDestroy(NativePtr);
                NativePtr = IntPtr.Zero;
            }
            m_BannerLoadListener?.Dispose();
            m_BannerLoadListener = null;
            base.Dispose();
        }

        ~IosBannerAd()
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

        internal void InvokeClickedEvent()
        {
            ThreadUtil.Post(state => OnClicked?.Invoke(this, EventArgs.Empty));
        }

        internal void InvokeRefreshedEvent(LoadErrorEventArgs args)
        {
            ThreadUtil.Post(state => OnRefreshed?.Invoke(this, args));
        }

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdCreate")]
        static extern IntPtr BannerAdCreate(string adUnitId, int width, int height);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdLoad")]
        static extern void BannerAdLoad(IntPtr bannerAdView, IntPtr bannerAdListener);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdDestroy")]
        static extern void BannerAdDestroy(IntPtr bannerAdView);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdSetPosition")]
        static extern void BannerAdSetPosition(IntPtr bannerAdView, int bannerAdAnchor, float offsetRatioX, float offsetRatioY);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdGetAdUnitId")]
        static extern string BannerAdGetAdUnitId(IntPtr bannerAdView);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdGetAdState")]
        static extern int BannerAdGetAdState(IntPtr nativePtr);
    }
}
#endif
