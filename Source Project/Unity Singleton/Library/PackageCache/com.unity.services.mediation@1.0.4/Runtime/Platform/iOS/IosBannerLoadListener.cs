#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosBannerLoadListener : IosNativeObject
    {
        delegate void LoadSuccessCallback(IntPtr bannerAd);
        delegate void LoadFailCallback(IntPtr bannerAd, int error, string message);
        delegate void ClickedCallback(IntPtr bannerAd);
        delegate void RefreshedCallback(IntPtr bannerAd, int error, string message);

        static readonly LoadSuccessCallback k_LoadSuccessCallback = LoadSuccess;
        static readonly LoadFailCallback    k_LoadFailCallback    = LoadFail;
        static readonly ClickedCallback     k_ClickedCallback     = Clicked;
        static readonly RefreshedCallback   k_RefreshedCallback   = Refreshed;


        public IosBannerLoadListener() : base(true)
        {
            NativePtr = BannerAdLoadDelegateCreate(
                k_LoadSuccessCallback,
                k_LoadFailCallback,
                k_ClickedCallback,
                k_RefreshedCallback);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            BannerAdLoadDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdLoadDelegateCreate")]
        static extern IntPtr BannerAdLoadDelegateCreate(LoadSuccessCallback startedCallback,
            LoadFailCallback loadFailedCallback, ClickedCallback clickedCallback, RefreshedCallback refreshedCallback);

        [DllImport("__Internal", EntryPoint = "UMSPBannerAdLoadDelegateDestroy")]
        static extern void BannerAdLoadDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(LoadSuccessCallback))]
        static void LoadSuccess(IntPtr ptr)
        {
            var bannerAd = Get<IosBannerAd>(ptr);
            bannerAd?.InvokeLoadedEvent();
        }

        [MonoPInvokeCallback(typeof(LoadFailCallback))]
        static void LoadFail(IntPtr ptr, int error, string message)
        {
            var bannerAd = Get<IosBannerAd>(ptr);
            bannerAd?.InvokeFailedLoadEvent(new LoadErrorEventArgs((LoadError)error, message));
        }

        [MonoPInvokeCallback(typeof(ClickedCallback))]
        static void Clicked(IntPtr ptr)
        {
            var bannerAd = Get<IosBannerAd>(ptr);
            bannerAd?.InvokeClickedEvent();
        }

        [MonoPInvokeCallback(typeof(RefreshedCallback))]
        static void Refreshed(IntPtr ptr, int error, string message)
        {
            var bannerAd = Get<IosBannerAd>(ptr);
            bannerAd?.InvokeRefreshedEvent(error < 0 ? null : new LoadErrorEventArgs((LoadError)error, message));
        }
    }
}
#endif
