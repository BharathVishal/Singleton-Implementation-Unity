#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosInterstitialLoadListener : IosNativeObject
    {
        delegate void LoadSuccessCallback(IntPtr interstitialAd);
        delegate void LoadFailCallback(IntPtr interstitialAd, int error, string message);

        static readonly LoadSuccessCallback k_LoadSuccessCallback = LoadSuccess;
        static readonly LoadFailCallback    k_LoadFailCallback    = LoadFail;

        public IosInterstitialLoadListener() : base(false)
        {
            NativePtr = InterstitialAdLoadDelegateCreate(k_LoadSuccessCallback, k_LoadFailCallback);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            InterstitialAdLoadDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdLoadDelegateCreate")]
        static extern IntPtr InterstitialAdLoadDelegateCreate(LoadSuccessCallback loadSuccessCallback, LoadFailCallback loadFailCallback);

        [DllImport("__Internal", EntryPoint = "UMSPInterstitialAdLoadDelegateDestroy")]
        static extern void InterstitialAdLoadDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(LoadSuccessCallback))]
        static void LoadSuccess(IntPtr ptr)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeLoadedEvent();
        }

        [MonoPInvokeCallback(typeof(LoadFailCallback))]
        static void LoadFail(IntPtr ptr, int error, string message)
        {
            var interstitialAd = Get<IosInterstitialAd>(ptr);
            interstitialAd?.InvokeFailedLoadEvent(new LoadErrorEventArgs((LoadError)error, message));
        }
    }
}
#endif
