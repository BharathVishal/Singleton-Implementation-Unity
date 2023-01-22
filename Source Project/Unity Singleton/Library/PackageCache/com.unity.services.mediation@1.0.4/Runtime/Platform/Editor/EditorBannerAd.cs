#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Services.Mediation.Platform
{
    class EditorBannerAd : IPlatformBannerAd
    {
        const string k_PrefabPath = "Packages/com.unity.services.mediation/Runtime/Platform/Editor/TestAds/MockBanner.prefab";

        MockBanner m_MockBannerAd;

        public EditorBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2())
        {
            GameObject mockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_PrefabPath);
            var adGameObject = Object.Instantiate(mockPrefab);
            Object.DontDestroyOnLoad(adGameObject);
            adGameObject.hideFlags = HideFlags.HideInHierarchy;

            m_MockBannerAd = adGameObject.GetComponent<MockBanner>();
            m_MockBannerAd.AdUnitId = adUnitId;
            m_MockBannerAd.Size = size;
            m_MockBannerAd.SetPosition(anchor, positionOffset);

            m_MockBannerAd.OnLoaded     += (sender, args) => OnLoaded?.Invoke(this, args);
            m_MockBannerAd.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_MockBannerAd.OnClicked    += (sender, args) => OnClicked?.Invoke(this, args);
            m_MockBannerAd.OnRefreshed  += (sender, args) => OnRefreshed?.Invoke(this, args);
        }

#pragma warning disable 67
        public event EventHandler OnLoaded;
        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;
        public event EventHandler OnClicked;
        public event EventHandler<LoadErrorEventArgs> OnRefreshed;
#pragma warning restore 67

        public AdState AdState => m_MockBannerAd.AdState;

        public string AdUnitId => m_MockBannerAd.AdUnitId;

        public BannerAdSize Size => m_MockBannerAd.Size;

        public void SetPosition(BannerAdAnchor anchor, Vector2 positionOffset = new Vector2())
        {
            m_MockBannerAd.SetPosition(anchor, positionOffset);
        }

        public void Load()
        {
            m_MockBannerAd.Load();
        }

        public void Dispose()
        {
            m_MockBannerAd.Dispose();
        }
    }
}
#endif
