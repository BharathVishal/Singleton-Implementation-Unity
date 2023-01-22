using System;
using UnityEngine.UIElements;

namespace Unity.Services.Mediation.Settings.Editor
{
    interface IAdapterSettings : IDisposable
    {
        string AdapterId { get; }

        ReloadableUserSetting<string> InstalledVersion { get; }

        void OnAdapterSettingsGui(string searchContext, VisualElement root);
    }
}
