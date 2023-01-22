using Unity.Services.Core.Editor;

namespace Unity.Services.Mediation.Settings.Editor
{
    class MediationServiceEnabler : EditorGameServiceFlagEnabler
    {
        static bool s_Enabled;

        protected override string FlagName { get; } = "mediation";

        protected override void EnableLocalSettings()
        {
            s_Enabled = true;
            MediationEditorService.RefreshGameId();
        }

        protected override void DisableLocalSettings()
        {
            s_Enabled = false;
        }

        public override bool IsEnabled()
        {
            return s_Enabled;
        }
    }
}
