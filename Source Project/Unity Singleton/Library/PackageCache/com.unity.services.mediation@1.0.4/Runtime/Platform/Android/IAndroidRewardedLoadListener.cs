#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    interface IAndroidRewardedLoadListener
    {
        void onRewardedLoaded(AndroidJavaObject rewardedAd);
        void onRewardedFailedLoad(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg);
    }
}
#endif
