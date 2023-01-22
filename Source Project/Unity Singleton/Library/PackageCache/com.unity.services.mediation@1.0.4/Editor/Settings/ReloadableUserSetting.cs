using System;
using UnityEditor;
using UnityEditor.SettingsManagement;

namespace Unity.Services.Mediation.Settings.Editor
{
    class ReloadableUserSetting<T> : UserSetting<T>, IDisposable
    {
        public ReloadableUserSetting(UnityEditor.SettingsManagement.Settings settings, string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(settings, key, value, scope)
        {
            settings.afterSettingsSaved += Reload;
        }

        public ReloadableUserSetting(UnityEditor.SettingsManagement.Settings settings, string repository, string key, T value, SettingsScope scope = SettingsScope.Project)
            : base(settings, repository, key, value, scope)
        {
            settings.afterSettingsSaved += Reload;
        }

        public void Reload()
        {
            value = settings.Get(key, scope, defaultValue);
        }

        public void Dispose()
        {
            settings.afterSettingsSaved -= Reload;
        }
    }
}
