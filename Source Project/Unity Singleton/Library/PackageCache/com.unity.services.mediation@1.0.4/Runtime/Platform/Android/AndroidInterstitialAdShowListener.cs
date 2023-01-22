#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidInterstitialAdShowListener : AndroidJavaProxy, IAndroidInterstitialShowListener
    {
        IAndroidInterstitialShowListener m_Listener;
        public AndroidInterstitialAdShowListener(IAndroidInterstitialShowListener listener) : base("com.unity3d.mediation.IInterstitialAdShowListener")
        {
            m_Listener = listener;
        }

        public void onInterstitialShowed(AndroidJavaObject interstitialAd)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialShowed(interstitialAd));
        }

        public void onInterstitialClicked(AndroidJavaObject interstitialAd)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialClicked(interstitialAd));
        }

        public void onInterstitialClosed(AndroidJavaObject interstitialAd)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialClosed(interstitialAd));
        }

        public void onInterstitialFailedShow(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialFailedShow(interstitialAd, error, msg));
        }
    }
}
#endif
