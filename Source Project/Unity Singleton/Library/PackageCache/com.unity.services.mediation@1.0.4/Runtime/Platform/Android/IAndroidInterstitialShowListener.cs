#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    interface IAndroidInterstitialShowListener
    {
        void onInterstitialShowed(AndroidJavaObject interstitialAd);
        void onInterstitialClicked(AndroidJavaObject interstitialAd);
        void onInterstitialClosed(AndroidJavaObject interstitialAd);
        void onInterstitialFailedShow(AndroidJavaObject interstitialAd, AndroidJavaObject error, string msg);
    }
}
#endif
