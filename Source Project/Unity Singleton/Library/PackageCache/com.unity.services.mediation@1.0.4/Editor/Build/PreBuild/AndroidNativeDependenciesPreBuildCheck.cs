#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MobileDependencyResolver.Utils.Editor;
using Unity.Services.Mediation.Adapters.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    class AndroidNativeDependenciesPreBuildCheck : IPreprocessBuildWithReport
    {
        string m_PluginFolder;
        string m_GradleTemplatePath;
        string m_InvalidPluginFiles;

        const string k_GradleFile = "mainTemplate.gradle";
        const string k_InvalidSpecs = "It looks like some dependencies are outdated or missing. This might lead to a broken build. To solve this, double check the Mediation settings under Project Settings.";
        const string k_ResolveQuestion = "You can fix this issue by resolving dependencies again. Would you like to do that now?";
        const string k_InvalidTemplate = "It looks like 'mainTemplate.gradle' was not patched correctly. " + k_ResolveQuestion;
        const string k_MobileDependencyResolverNotFound = "Mobile Dependency Resolver not detected in this project.\nThis might cause dependencies to be missing in your build.";
        const string k_IncompatibleSDKPackageVersions = "The SDK has resolved to an incompatible version with the Package. This may result in unexpected behaviour. To avoid this, include the " + k_GradleFile + " file by checking the checkbox 'Custom Main Gradle Template' in Edit > Project Settings > Player > Publishing Settings.";
        const string k_GradleTemplateWarningDescription = "This project's Mobile Dependency Resolver settings indicate that libraries will be downloaded in the editor. This resolution method might cause the editor to hang until resolution is complete.\n To change this go to Assets > Android Resolver > Settings and look for \"Patch mainTemplate.gradle\".";

        enum UserAction
        {
            CancelBuild,
            ResolveDependenciesAndContinue
        }

        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            m_PluginFolder = Path.Combine("Assets", "Plugins", "Android");
            m_GradleTemplatePath = Path.Combine(m_PluginFolder, k_GradleFile);
            m_InvalidPluginFiles = "It looks like there are a few files missing from '" + m_PluginFolder + "'. " + k_ResolveQuestion;

            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }
            // Since we display a dialog for the user, set noDialog to true if in batch mode
            ValidateDependencies(UnityEditorInternal.InternalEditorUtility.inBatchMode);
            MediationSdkInfo.Apply(true);
        }

        public void ValidateDependencies(bool noDialog)
        {
            if (!MobileDependencyResolverUtils.IsPresent)
            {
                Debug.LogWarning(k_MobileDependencyResolverNotFound);
                return;
            }

            var sdkInfo = MediationSdkInfo.GetSdkInfo();
            // Calling GetInstalledAdapters() should refresh the xml dependency file if needed.
            var installedAdapters = MediationSdkInfo.GetInstalledAdapters();
            var specs = MobileDependencyResolverUtils.GetPackageSpecs();

            //Check xml generation
            if (!ValidateSpecs(sdkInfo, installedAdapters, specs))
            {
                //Should not happen, since a call to GetInstalledAdapters() refreshes outdated dependencies in the xml.
                if (!DisplayIssueDetectedDialog(noDialog, k_InvalidSpecs, UserAction.CancelBuild))
                    return;
            }

            //Check resolved artifacts
            if (MobileDependencyResolverUtils.GradleTemplateEnabled)
            {
                if (!ValidateTemplateFile(sdkInfo, installedAdapters, m_GradleTemplatePath))
                {
                    if (!DisplayIssueDetectedDialog(noDialog, k_InvalidTemplate, UserAction.ResolveDependenciesAndContinue))
                        return;
                }
            }
            else
            {
                if (!ValidatePluginsFolder(sdkInfo, installedAdapters, m_PluginFolder))
                {
                    if (!DisplayIssueDetectedDialog(noDialog, m_InvalidPluginFiles, UserAction.ResolveDependenciesAndContinue))
                        return;
                }

                if (!IsSdkCompatibleWithPackage())
                {
                    if (!DisplayIssueDetectedDialog(noDialog, k_IncompatibleSDKPackageVersions, UserAction.CancelBuild))
                        return;
                }
            }
        }

        bool IsSdkCompatibleWithPackage()
        {
            var sdkPrefix = "com.unity3d.mediation.mediation-sdk-";
            var metaExtension = "meta";
            Path.Combine();
            var sdkInfo = MediationSdkInfo.GetSdkInfo();
            var pluginFiles = Directory.GetFiles(m_PluginFolder);
            var sdkFullFileName = pluginFiles.FirstOrDefault(x => Path.GetFileName(x).StartsWith(sdkPrefix) && !x.EndsWith(metaExtension));
            if (sdkFullFileName == null) return false;

            var sdkFileName = Path.GetFileNameWithoutExtension(sdkFullFileName);
            var sdkVersionString = sdkFileName.Replace(sdkPrefix, "");
            var sdkVersion = Version.Parse(sdkVersionString);
            var packageVersion = Version.Parse(sdkInfo.SdkVersion);
            //Logic subject to change at GA! Change to check major only.
            return sdkVersion.Major == packageVersion.Major && sdkVersion.Minor == packageVersion.Minor;
        }

        bool DisplayIssueDetectedDialog(bool noDialog, string issue, UserAction action)
        {
            switch (action)
            {
                case UserAction.CancelBuild:
                    if (noDialog || EditorUtility.DisplayDialog("Dependency issue detected",
                        "Issue with Android dependencies detected:\n" + issue +
                        "\nWould you like to cancel the build?", "Cancel build", "Ignore"))
                    {
                        throw new BuildFailedException("Build canceled");
                    }
                    return false;
                case UserAction.ResolveDependenciesAndContinue:
                    if (noDialog || EditorUtility.DisplayDialog("Dependency issue detected",
                        "Issue with Android dependencies detected:\n" + issue +
                        "\nWould you like to resolve dependencies before building?", "Resolve", "Ignore"))
                    {
                        if (!MobileDependencyResolverUtils.MainTemplateEnabled)
                        {
                            EditorUtility.DisplayProgressBar("", "Resolving...", 0.20f);
                            Debug.LogWarning(k_GradleTemplateWarningDescription);
                        }

                        MobileDependencyResolverUtils.ResolveSync(true);
                        return true;
                    }
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        /// <summary>
        /// Validate specs captured by PlayServicesResolver.
        /// They should be aligned with the current SDK version and installed adapters.
        /// </summary>
        bool ValidateSpecs(SdkInfo sdkInfo, List<AdapterInfo> installedAdapters, IList<KeyValuePair<string, string>> specs)
        {
            var sdkSpec = specs
                .Select(pair => pair.Key)
                .Where(s => s.StartsWith(sdkInfo.AndroidArtifact))
                .ToArray();

            //Check if SDK missing (or included multiple times for some reason)
            if (sdkSpec.Length != 1)
            {
                return false;
            }

            //Check if we have valid version
            if (sdkSpec.First() != $"{sdkInfo.AndroidArtifact}:{VersionInfo.OptimisticVersion(SemanticVersioningType.Maven,sdkInfo.SdkVersion)}")
            {
                return false;
            }

            foreach (var adapter in installedAdapters)
            {
                var adapterSpec = specs
                    .Select(pair => pair.Key)
                    .Where(s => s.StartsWith(adapter.AndroidArtifact))
                    .ToArray();

                //Check if adapter missing (or included multiple times for some reason)
                if (adapterSpec.Length != 1)
                {
                    return false;
                }

                //Check if we have valid version
                var adapterVersion = adapterSpec.First().Substring(adapterSpec.First().LastIndexOf(':') + 1);
                if (adapterVersion != VersionInfo.OptimisticVersion(SemanticVersioningType.Maven, sdkInfo.SdkVersion) &&
                    !Array.Exists(adapter.Versions, x => x.Identifier == adapterVersion))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Ensures the SDK artifact and all installed adapters are included in the template file.
        /// </summary>
        bool ValidateTemplateFile(SdkInfo sdkInfo, List<AdapterInfo> installedAdapters, string templateFile)
        {
            if (!File.Exists(templateFile))
                return false;
            var templateFileContent = File.ReadAllText(templateFile);
            var isValid = ValidateTemplateDependency(sdkInfo.AndroidArtifact, sdkInfo.SdkVersion, templateFileContent);
            foreach (var adapter in installedAdapters)
            {
                isValid = isValid && ValidateTemplateDependency(adapter.AndroidArtifact, sdkInfo.SdkVersion, templateFileContent);
                if (!isValid)
                {
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Ensures the template file contains the artifact with the specific version.
        /// E.g. template file should contain com.unity3d.mediation:mediation-sdk:1.0.0
        /// </summary>
        bool ValidateTemplateDependency(string artifactName, string version, string templateFileContent)
        {
            return templateFileContent.Contains($"{artifactName}:{VersionInfo.OptimisticVersion(SemanticVersioningType.Maven, version)}");
        }

        /// <summary>
        /// Ensures that the main SDK artifact `com.unity3d.mediation:mediation-sdk` and installed adapters are
        /// included in the plugins folder, and that they have valid versions.
        /// </summary>
        bool ValidatePluginsFolder(SdkInfo sdkInfo, List<AdapterInfo> installedAdapters, string pluginsFolder)
        {
            if (!Directory.Exists(pluginsFolder))
            {
                return false;
            }

            var plugins = Directory.GetFiles(m_PluginFolder)
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();

            var isValid = ValidatePluginFile(sdkInfo.AndroidArtifact, sdkInfo.SdkVersion, plugins);
            foreach (var adapter in installedAdapters)
            {
                isValid = isValid && ValidatePluginFile(adapter.AndroidArtifact, adapter.InstalledVersion.Identifier, plugins);
            }
            return isValid;
        }

        /// <summary>
        /// Ensures the list of files contains the plugin associated with an artifact.
        /// E.g. for artifact `com.unity3d.mediation:mediation-sdk` version `1.0.0`,
        /// there should be a `com.unity3d.mediation.mediation-sdk-1.0.0` in the files list
        /// </summary>
        bool ValidatePluginFile(string artifactName, string version, IEnumerable<string> files)
        {
            var filePrefix = $"{artifactName.Replace(":", ".")}";
            return files.Count(x => x.StartsWith(filePrefix)) > 0;
        }
    }
}
#endif
