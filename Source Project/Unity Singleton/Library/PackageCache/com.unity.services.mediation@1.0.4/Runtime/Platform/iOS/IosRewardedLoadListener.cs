#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosRewardedLoadListener : IosNativeObject
    {
        delegate void LoadSuccessCallback(IntPtr rewardedAd);
        delegate void LoadFailCallback(IntPtr rewardedAd, int error, string message);

        static readonly LoadSuccessCallback k_LoadSuccessCallback = LoadSuccess;
        static readonly LoadFailCallback    k_LoadFailCallback    = LoadFail;

        public IosRewardedLoadListener() : base(false)
        {
            NativePtr = RewardedAdLoadDelegateCreate(k_LoadSuccessCallback, k_LoadFailCallback);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            RewardedAdLoadDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdLoadDelegateCreate")]
        static extern IntPtr RewardedAdLoadDelegateCreate(LoadSuccessCallback loadSuccessCallback, LoadFailCallback loadFailCallback);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdLoadDelegateDestroy")]
        static extern void RewardedAdLoadDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(LoadSuccessCallback))]
        static void LoadSuccess(IntPtr ptr)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeLoadedEvent();
        }

        [MonoPInvokeCallback(typeof(LoadFailCallback))]
        static void LoadFail(IntPtr ptr, int error, string message)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeFailedLoadEvent(new LoadErrorEventArgs((LoadError)error, message));
        }
    }
}
#endif
