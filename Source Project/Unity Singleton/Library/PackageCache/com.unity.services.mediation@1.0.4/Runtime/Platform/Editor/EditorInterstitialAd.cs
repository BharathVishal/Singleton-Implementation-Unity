#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Services.Mediation.Platform
{
    class EditorInterstitialAd : IPlatformInterstitialAd
    {
#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_PrefabPath = @"Assets/UnityMonetization/Runtime/Platform/Editor/TestAds/MockInterstitial.prefab";
#else
        const string k_PrefabPath = "Packages/com.unity.services.mediation/Runtime/Platform/Editor/TestAds/MockInterstitial.prefab";
#endif

        private MockInterstitial m_MockInterstitial;

        public EditorInterstitialAd(string adUnitId)
        {
            GameObject mockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_PrefabPath);
            var adGameObject = Object.Instantiate(mockPrefab);
            Object.DontDestroyOnLoad(adGameObject);
            adGameObject.hideFlags = HideFlags.HideInHierarchy;

            m_MockInterstitial = adGameObject.GetComponent<MockInterstitial>();
            m_MockInterstitial.AdUnitId = adUnitId;

            m_MockInterstitial.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_MockInterstitial.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_MockInterstitial.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_MockInterstitial.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_MockInterstitial.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_MockInterstitial.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
        }

#pragma warning disable 67
        public event EventHandler OnLoaded;

        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        public event EventHandler OnShowed;

        public event EventHandler OnClicked;

        public event EventHandler OnClosed;

        public event EventHandler<ShowErrorEventArgs> OnFailedShow;
#pragma warning restore 67

        public AdState AdState => m_MockInterstitial.AdState;

        public string AdUnitId => m_MockInterstitial.AdUnitId;

        public void Load()
        {
            m_MockInterstitial.Load();
        }

        public void Show()
        {
            m_MockInterstitial.Show();
        }

        public void Dispose()
        {
            m_MockInterstitial.Dispose();
        }
    }
}
#endif
