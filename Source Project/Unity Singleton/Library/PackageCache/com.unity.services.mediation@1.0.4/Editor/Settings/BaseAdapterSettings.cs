using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    abstract class BaseAdapterSettings : IAdapterSettings
    {
        protected BaseAdapterSettings(string adapterId, bool enabled = false)
        {
            Initialize(adapterId, enabled);
        }

        protected BaseAdapterSettings()
        {
            Initialize(AdapterId);
        }

        void Initialize(string adapterId, bool enabled = false)
        {
            InstalledVersion = new ReloadableUserSetting<string>(MediationSettingsProvider.instance, $"{adapterId}.version", "");
        }

        public ReloadableUserSetting<string> InstalledVersion { get; private set; }
        public abstract string AdapterId { get; }
        public virtual void OnAdapterSettingsGui(string searchContext, VisualElement root) {}

        public void Save()
        {
            MediationSettingsProvider.instance.Save();
        }

        public virtual void Dispose()
        {
            InstalledVersion.Dispose();
        }
    }
}
