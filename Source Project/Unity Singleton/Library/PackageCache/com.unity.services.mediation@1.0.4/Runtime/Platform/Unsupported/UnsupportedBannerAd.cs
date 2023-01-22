//#if !UNITY_ANDROID && !UNITY_IOS
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedBannerAd : IPlatformBannerAd
    {
        const string k_PackageTag = "Mediation";
        const string k_UnsupportedWarning = "{0}: Unity Mediation is not supported on this platform";

        public void Dispose()
        {
        }

#pragma warning disable 67
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnClicked;
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;
#pragma warning restore 67

        public AdState AdState => AdState.Unloaded;
        public string AdUnitId { get; } = String.Empty;
        public BannerAdSize Size { get; } = null;

        public UnsupportedBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2())
        {
            Debug.unityLogger.LogWarning(k_PackageTag, string.Format(k_UnsupportedWarning, System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2())
        {
            Debug.unityLogger.LogWarning(k_PackageTag, string.Format(k_UnsupportedWarning, System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        public void Load()
        {
            Debug.unityLogger.LogWarning(k_PackageTag, string.Format(k_UnsupportedWarning, System.Reflection.MethodBase.GetCurrentMethod().Name));
        }
    }
}

//#endif
