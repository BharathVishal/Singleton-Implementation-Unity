using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MintegralSettings : BaseAdapterSettings
    {
        const string k_AdapterTemplate = @"Packages/com.unity.services.mediation/Editor/Settings/Adapters/Layout/MintegralTemplate.uxml";
        const string k_EarlyAccessUrl = @"https://create.unity.com/mediation-early-features";
        public override string AdapterId => "mintegral-adapter";

        public override void OnAdapterSettingsGui(string searchContext, VisualElement rootElement)
        {
            var adapterTemplate  = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AdapterTemplate);
            var holder = new VisualElement();
            adapterTemplate.CloneTree(rootElement);
            rootElement.Q<VisualElement>("EarlyAccessMintegralContainer").AddManipulator(new Clickable(LinkToSourceConfig));
        }

        static void LinkToSourceConfig()
        {
            Application.OpenURL(k_EarlyAccessUrl);
        }
    }
}
