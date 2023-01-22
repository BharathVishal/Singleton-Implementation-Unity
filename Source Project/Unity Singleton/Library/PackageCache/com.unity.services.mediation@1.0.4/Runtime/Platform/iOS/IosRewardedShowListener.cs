#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosRewardedShowListener : IosNativeObject
    {
        delegate void ShownCallback(IntPtr rewardedAd);
        delegate void ClickedCallback(IntPtr rewardedAd);
        delegate void ClosedCallback(IntPtr rewardedAd);
        delegate void FailedShowCallback(IntPtr rewardedAd, int error, string message);
        delegate void UserRewardedCallback(IntPtr rewardedAd, string type, string amount);

        static readonly ShownCallback        k_ShownCallback        = Shown;
        static readonly ClickedCallback      k_ClickedCallback      = Clicked;
        static readonly ClosedCallback       k_ClosedCallback       = Closed;
        static readonly FailedShowCallback   k_FailedShowCallback   = FailedShow;
        static readonly UserRewardedCallback k_UserRewardedCallback = UserRewarded;

        public IosRewardedShowListener() : base(false)
        {
            NativePtr = RewardedAdShowDelegateCreate(
                k_ShownCallback,
                k_ClickedCallback,
                k_ClosedCallback,
                k_FailedShowCallback,
                k_UserRewardedCallback);
        }

        public override void Dispose()
        {
            if (NativePtr == IntPtr.Zero) return;
            RewardedAdShowDelegateDestroy(NativePtr);
            NativePtr = IntPtr.Zero;
        }

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdShowDelegateCreate")]
        static extern IntPtr RewardedAdShowDelegateCreate(ShownCallback startedCallback, ClickedCallback clickedCallback,
            ClosedCallback finishedCallback, FailedShowCallback failedShowCallback, UserRewardedCallback userRewardedCallback);

        [DllImport("__Internal", EntryPoint = "UMSPRewardedAdShowDelegateDestroy")]
        static extern void RewardedAdShowDelegateDestroy(IntPtr ptr);

        [MonoPInvokeCallback(typeof(ShownCallback))]
        static void Shown(IntPtr ptr)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeShownEvent();
        }

        [MonoPInvokeCallback(typeof(ClickedCallback))]
        static void Clicked(IntPtr ptr)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeClickedEvent();
        }

        [MonoPInvokeCallback(typeof(ClosedCallback))]
        static void Closed(IntPtr ptr)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeClosedEvent();
        }

        [MonoPInvokeCallback(typeof(FailedShowCallback))]
        static void FailedShow(IntPtr ptr, int error, string message)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeFailedShowEvent(new ShowErrorEventArgs((ShowError)error, message));
        }

        [MonoPInvokeCallback(typeof(UserRewardedCallback))]
        static void UserRewarded(IntPtr ptr, string type, string amount)
        {
            var rewardedAd = Get<IosRewardedAd>(ptr);
            rewardedAd?.InvokeUserRewardedEvent(new RewardEventArgs(type, amount));
        }
    }
}
#endif
