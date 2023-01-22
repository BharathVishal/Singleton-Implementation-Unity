#if UNITY_ANDROID
using System;
using System.IO;
using System.Linq;
using Unity.Services.Mediation.Settings.Editor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    class AdMobGradleVersionPreBuildCheck : IPreprocessBuildWithReport
    {
        const string k_AdMobWarning = "Unity Mediation: AdMob adapter is enabled and requires gradle version 5.6.4" +
            " to build succesfully. Current Version : {0}";

        const string k_GradleLibPathSuffix = "/lib";
        const string k_GradleFilePrefix = "gradle-";
        const string k_TrimmedFileSuffix = ".jar";

        readonly Version k_MinimumGradleVersion = new Version("5.6.4");

        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            // Check if AdMob is Installed
            var adMobSettings = new AdMobSettings();
            if (string.IsNullOrEmpty(adMobSettings.InstalledVersion))
                return;

            if (!IsGradleVersionSufficient(out string detectedVersion))
            {
                Debug.LogWarning(string.Format(k_AdMobWarning, detectedVersion));
            }
        }

        private bool IsGradleVersionSufficient(out string detectedVersion)
        {
            /*
             * Gradle Files Look like the following :
             *     gradle-core-5.6.4.jar
             *     gradle-kotlin-dsl-6.8.3.jar
             *
             *  Retrieve version as characters after last '-' and before '.jar'
             */

            string gradleLibPath = AndroidExternalToolsSettings.gradlePath + k_GradleLibPathSuffix;
            string gradleLibFile = Directory.GetFiles(gradleLibPath)
                ?.FirstOrDefault(file => file.Substring(gradleLibPath.Length + 1).StartsWith(k_GradleFilePrefix))
                ?.Substring(gradleLibPath.Length + 1);

            // Cannot find gradle file for version parsing.
            if (gradleLibFile == null)
            {
                detectedVersion = null;
                return false;
            }

            string[] gradleVersionSuffixSplit = gradleLibFile.Split('-');
            string gradleVersionSuffix = gradleVersionSuffixSplit[gradleVersionSuffixSplit.Length - 1];
            string gradleVersion = gradleVersionSuffix.Replace(k_TrimmedFileSuffix, string.Empty);

            detectedVersion = gradleVersion;
            if (Version.TryParse(gradleVersion, out Version version))
            {
                return version.CompareTo(k_MinimumGradleVersion) >= 0;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif
