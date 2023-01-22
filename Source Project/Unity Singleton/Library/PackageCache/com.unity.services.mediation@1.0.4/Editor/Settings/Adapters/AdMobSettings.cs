using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class AdMobSettings : BaseAdapterSettings
    {
#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_AdapterTemplate = @"Assets/UnityMonetization/Editor/Settings/Adapters/Layout/AdMobTemplate.uxml";
#else
        const string k_AdapterTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Adapters/Layout/AdMobTemplate.uxml";
#endif
        public override string AdapterId => "admob-adapter";

        public string AdMobAppIdAndroid
        {
            get => m_AdMobAppIdAndroid.value;
            set => m_AdMobAppIdAndroid.value = value;
        }

        public string AdMobAppIdIos
        {
            get => m_AdMobAppIdIos.value;
            set => m_AdMobAppIdIos.value = value;
        }

        internal ReloadableUserSetting<string> m_AdMobAppIdAndroid;
        internal ReloadableUserSetting<string> m_AdMobAppIdIos;

        TextField m_txtAdMobAppIdAndroid;
        TextField m_txtAdMobAppIdIos;

        public AdMobSettings()
        {
            m_AdMobAppIdAndroid = new ReloadableUserSetting<string>(MediationSettingsProvider.instance,
                $"{AdapterId}.app-id.android", "");
            m_AdMobAppIdIos = new ReloadableUserSetting<string>(MediationSettingsProvider.instance,
                $"{AdapterId}.app-id.ios", "");
        }

        public override void OnAdapterSettingsGui(string searchContext, VisualElement rootElement)
        {
            VisualTreeAsset adapterTemplate  = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdapterTemplate);
            adapterTemplate.CloneTree(rootElement);

            m_txtAdMobAppIdAndroid = rootElement.Q<TextField>("AndroidAdmobID");
            m_txtAdMobAppIdIos     = rootElement.Q<TextField>("IosAdmobID");

            m_txtAdMobAppIdAndroid.value = m_AdMobAppIdAndroid.value;
            m_txtAdMobAppIdIos.value     = m_AdMobAppIdIos.value;

            m_txtAdMobAppIdAndroid.RegisterValueChangedCallback((changeEvent) => AdMobAppIdAndroid = changeEvent.newValue);
            m_txtAdMobAppIdIos.RegisterValueChangedCallback((changeEvent) => AdMobAppIdIos     = changeEvent.newValue);
        }

        public override void Dispose()
        {
            base.Dispose();
            m_AdMobAppIdAndroid.Dispose();
            m_AdMobAppIdIos.Dispose();
        }
    }
}
