#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidInterstitialAdLoadListener : AndroidJavaProxy, IAndroidInterstitialLoadListener
    {
        IAndroidInterstitialLoadListener m_Listener;
        public AndroidInterstitialAdLoadListener(IAndroidInterstitialLoadListener listener) : base("com.unity3d.mediation.IInterstitialAdLoadListener")
        {
            m_Listener = listener;
        }

        [UnityEngine.Scripting.Preserve]
        public void onInterstitialLoaded(AndroidJavaObject interstitialAd)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialLoaded(interstitialAd));
        }

        [UnityEngine.Scripting.Preserve]
        public void onInterstitialFailedLoad(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg)
        {
            ThreadUtil.Post(state => m_Listener.onInterstitialFailedLoad(interstitialAd, error, msg));
        }
    }
}
#endif
