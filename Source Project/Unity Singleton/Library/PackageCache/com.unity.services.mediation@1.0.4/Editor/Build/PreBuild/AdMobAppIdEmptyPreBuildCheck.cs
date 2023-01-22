#if UNITY_ANDROID || UNITY_IOS
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    class AdMobAppIdEmptyPreBuildCheck : IPreprocessBuildWithReport
    {
        const string k_AdMobWarning = "Unity Mediation: AdMob adapter is enabled, but AdMob application identifier is not set. " +
            "You can configure it under 'Project Settings/" + MediationServiceIdentifier.k_PackageDisplayName + "'.";

        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            var adMobSettings = new AdMobSettings();
            if (string.IsNullOrEmpty(adMobSettings.InstalledVersion))
                return;

            if (report.summary.platform == BuildTarget.Android && string.IsNullOrWhiteSpace(adMobSettings.AdMobAppIdAndroid) ||
                report.summary.platform == BuildTarget.iOS && string.IsNullOrWhiteSpace(adMobSettings.AdMobAppIdIos))
            {
                Debug.LogWarning(k_AdMobWarning);
            }
        }
    }
}
#endif
