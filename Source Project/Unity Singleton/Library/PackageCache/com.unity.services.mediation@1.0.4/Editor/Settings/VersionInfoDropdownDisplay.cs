using Unity.Services.Mediation.Adapters.Editor;

namespace Unity.Services.Mediation.Settings.Editor
{
    class VersionInfoDropdownDisplay
    {
        public VersionInfo VersionInfo;
        public bool IsInstalled;

        string m_DisplayPrefix = "";
        const string k_UpdateToPrefix = "Update to ";
        const string k_InstallPrefix = "Install ";

        public VersionInfoDropdownDisplay(VersionInfo versionInfo, bool isInstalled)
        {
            VersionInfo = versionInfo;
            IsInstalled = isInstalled;
        }

        /// <summary>
        /// Sets internal values dependent on if it is the currently selected version or not
        /// </summary>
        public void NotifyCurrentlyInstalledVersion(string identifier)
        {
            if (!IsInstalled)
            {
                m_DisplayPrefix = k_InstallPrefix;
            }
            else if (identifier == VersionInfo.Identifier)
            {
                m_DisplayPrefix = "";
            }
            else
            {
                m_DisplayPrefix = k_UpdateToPrefix;
            }
        }

        public override string ToString()
        {
            return m_DisplayPrefix + VersionInfo.DisplayName;
        }
    }
}
