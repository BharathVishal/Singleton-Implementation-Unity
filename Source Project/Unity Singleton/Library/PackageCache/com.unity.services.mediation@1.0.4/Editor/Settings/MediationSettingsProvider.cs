using System.Collections.Generic;
using UnityEditor;
using Unity.Services.Core.Editor;
using Unity.Services.Mediation.Adapters.Editor;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MediationSettingsProvider : EditorGameServiceSettingsProvider
    {
        static UnityEditor.SettingsManagement.Settings s_SettingsInstance;

        public static UnityEditor.SettingsManagement.Settings instance
        {
            get
            {
                if (s_SettingsInstance == null)
                    s_SettingsInstance = new UnityEditor.SettingsManagement.Settings("com.unity.services.mediation");
                return s_SettingsInstance;
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new MediationSettingsProvider(GenerateProjectSettingsPath(new MediationServiceIdentifier().GetKey()), SettingsScope.Project);
        }

        MediationSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
            MediationEditorService.RefreshGameId();
            MediationSdkInfo.GetInstalledAdapters();
        }

        protected override IEditorGameService EditorGameService => EditorGameServiceRegistry.Instance.GetEditorGameService<MediationServiceIdentifier>();

        protected override string Title => EditorGameService.Name;
        protected override string Description => "Manage your ad network adapters and generate code snippets to implement ads in your game.";

        protected override VisualElement GenerateServiceDetailUI()
        {
            return MediationAdapterSettings.GenerateUIElementUI();
        }

        protected override VisualElement GenerateUnsupportedDetailUI()
        {
            return MediationAdapterSettings.GenerateUIElementUI();
        }
    }
}
