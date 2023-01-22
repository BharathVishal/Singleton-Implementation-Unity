using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class UnityAdsSettings : BaseAdapterSettings
    {
#if GAMEGROWTH_UNITY_MONETIZATION
        const string k_AdapterTemplate = @"Assets/UnityMonetization/Editor/Settings/Adapters/Layout/UnityAdsTemplate.uxml";
#else
        const string k_AdapterTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Adapters/Layout/UnityAdsTemplate.uxml";
#endif
        public override string AdapterId => "unityads-adapter";
        bool? m_UnityAdsDetected;

        public override void OnAdapterSettingsGui(string searchContext, VisualElement rootElement)
        {
            if (IsUnityAdsDetected())
            {
                VisualTreeAsset adapterTemplate  = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdapterTemplate);
                VisualElement holder = new VisualElement();

                adapterTemplate.CloneTree(rootElement);
            }
        }

        bool IsUnityAdsDetected()
        {
            if (!m_UnityAdsDetected.HasValue)
            {
                try
                {
                    m_UnityAdsDetected = Type.GetType("UnityEngine.Advertisements.Advertisement, UnityEngine.Advertisements") != null;
                }
                catch (Exception e)
                {
                    //TODO: analytics
                    Debug.LogException(e);
                    m_UnityAdsDetected = false;
                }
            }
            return m_UnityAdsDetected.Value;
        }
    }
}
