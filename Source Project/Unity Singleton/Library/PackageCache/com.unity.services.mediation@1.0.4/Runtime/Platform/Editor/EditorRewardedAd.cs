#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Services.Mediation.Platform
{
    class EditorRewardedAd : IPlatformRewardedAd
    {
#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_PrefabPath = @"Assets/UnityMonetization/Runtime/Platform/Editor/TestAds/MockRewarded.prefab";
#else
        const string k_PrefabPath = "Packages/com.unity.services.mediation/Runtime/Platform/Editor/TestAds/MockRewarded.prefab";
#endif

        private MockRewarded m_MockRewarded;

        public EditorRewardedAd(string adUnitId)
        {
            GameObject mockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(k_PrefabPath);
            var adGameObject = Object.Instantiate(mockPrefab);
            Object.DontDestroyOnLoad(adGameObject);
            adGameObject.hideFlags = HideFlags.HideInHierarchy;

            m_MockRewarded = adGameObject.GetComponent<MockRewarded>();
            m_MockRewarded.AdUnitId = adUnitId;

            m_MockRewarded.OnLoaded += (sender, args) => OnLoaded?.Invoke(this, args);
            m_MockRewarded.OnFailedLoad += (sender, args) => OnFailedLoad?.Invoke(this, args);
            m_MockRewarded.OnClicked += (sender, args) => OnClicked?.Invoke(this, args);
            m_MockRewarded.OnClosed += (sender, args) => OnClosed?.Invoke(this, args);
            m_MockRewarded.OnUserRewarded += (sender, args) => OnUserRewarded?.Invoke(this, args);
            m_MockRewarded.OnShowed += (sender, args) => OnShowed?.Invoke(this, args);
            m_MockRewarded.OnFailedShow += (sender, args) => OnFailedShow?.Invoke(this, args);
        }

#pragma warning disable 67
        public event EventHandler OnLoaded;

        public event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        public event EventHandler OnShowed;

        public event EventHandler OnClicked;

        public event EventHandler OnClosed;

        public event EventHandler<ShowErrorEventArgs> OnFailedShow;

        public event EventHandler<RewardEventArgs> OnUserRewarded;
#pragma warning restore 67

        public AdState AdState => m_MockRewarded.AdState;

        public string AdUnitId => m_MockRewarded.AdUnitId;

        public void Load()
        {
            m_MockRewarded.Load();
        }

        public void Show(RewardedAdShowOptions showOptions = null)
        {
            m_MockRewarded.Show(showOptions);
        }

        public void Dispose()
        {
            m_MockRewarded.Dispose();
        }
    }
}
#endif
