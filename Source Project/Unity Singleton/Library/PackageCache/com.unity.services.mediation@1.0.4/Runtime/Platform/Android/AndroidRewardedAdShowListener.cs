#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidRewardedAdShowListener : AndroidJavaProxy, IAndroidRewardedShowListener
    {
        IAndroidRewardedShowListener m_Listener;
        public AndroidRewardedAdShowListener(IAndroidRewardedShowListener listener) : base("com.unity3d.mediation.IRewardedAdShowListener")
        {
            m_Listener = listener;
        }

        public void onRewardedShowed(AndroidJavaObject rewardedAd)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedShowed(rewardedAd));
        }

        public void onRewardedClicked(AndroidJavaObject rewardedAd)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedClicked(rewardedAd));
        }

        public void onRewardedClosed(AndroidJavaObject rewardedAd)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedClosed(rewardedAd));
        }

        public void onRewardedFailedShow(AndroidJavaObject rewardedAd, AndroidJavaObject error, string msg)
        {
            ThreadUtil.Post(state => m_Listener.onRewardedFailedShow(rewardedAd, error, msg));
        }

        public void onUserRewarded(AndroidJavaObject rewardedAd, AndroidJavaObject reward)
        {
            ThreadUtil.Post(state => m_Listener.onUserRewarded(rewardedAd, reward));
        }
    }
}
#endif
