using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MobileDependencyResolver.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Mediation.Adapters.Editor
{
    class AdaptersDependencyGenerator
    {
        const string k_FileName = "MediationAdapterDependencies.xml";
        const string k_DefaultFolder = "Assets/Editor";
        const string k_DefaultPath = k_DefaultFolder + "/" + k_FileName;

        const string k_IdentifierKey = "mediation-identifier";
        const string k_VersionKey = "mediation-version";

#if UNITY_MEDIATION_STAGING
        const string k_AndroidArtifactoryURL = @"https://unity3ddist.jfrog.io/artifactory/unity-mediation-mvn-stg-local/";
        const string k_IOSArtifactoryURL = @"'git@github.com:Unity-Technologies/mz-liveops-cocoapods.git'";
#else // Prod
        const string k_AndroidArtifactoryURL = @"https://unity3ddist.jfrog.io/artifactory/unity-mediation-mvn-prod-local/";
        const string k_IOSArtifactoryURL = @"'https://github.com/Unity-Technologies/unity-mediation-cocoapods-prod.git'";
#endif

        string m_DependenciesPath;
        Regex m_MavenRegex;

        public virtual event Action AdaptersChanged;

        string GetDependenciesPath(bool createPath = true)
        {
            if (m_DependenciesPath != null)
            {
                if (File.Exists(m_DependenciesPath))
                {
                    m_DependenciesPath = EnsureDependenciesFileInEditorFolder(m_DependenciesPath);
                    return m_DependenciesPath;
                }
            }
            if (File.Exists(k_DefaultPath))
            {
                m_DependenciesPath = k_DefaultPath;
                return m_DependenciesPath;
            }
            var files = Directory.GetFiles("Assets", k_FileName, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                var file = files[0];
                file = EnsureDependenciesFileInEditorFolder(file);
                m_DependenciesPath = file;
                return m_DependenciesPath;
            }

            if (createPath)
            {
                m_DependenciesPath = CreateDependencyPath();
            }
            return m_DependenciesPath;
        }

        string CreateDependencyPath()
        {
            Directory.CreateDirectory(k_DefaultFolder);

            AdapterInfo unityAdsAdapterInfo = MediationSdkInfo.GetAllAdapters().Find((x) => x.Identifier == "unityads-adapter");
            if (unityAdsAdapterInfo.InstalledVersion?.Identifier == null)
            {
                unityAdsAdapterInfo.InstalledVersion = unityAdsAdapterInfo.Versions.First();
            }
            InstallOverrideAdapters(k_DefaultPath, new List<AdapterInfo>() {unityAdsAdapterInfo});
            return m_DependenciesPath;
        }

        internal string GenerateXmlContentWithAllDependencies(List<AdapterInfo> adapters)
        {
            var sdkInfo = MediationSdkInfo.GetSdkInfo();
            var repoList = new List<string>();

            foreach (var adapterInfo in adapters)
            {
                foreach (var repository in adapterInfo.Repositories)
                {
                    repoList.Add(repository);
                }
            }

            repoList = repoList.Distinct().ToList();

            return (new XComment(" auto-generated. do not modify by hand. ") + Environment.NewLine +
                new XElement("dependencies",
                    new XElement("androidPackages",
                        new XElement("repositories",
                            new XElement("repository", k_AndroidArtifactoryURL),
                            repoList.Select(repo => new XElement("repository", repo))
                        ),
                        new XComment(" Mediation Android SDK "),
                        new XElement("androidPackage",
                            new XAttribute("spec", $"{sdkInfo.AndroidArtifact}:{VersionInfo.OptimisticVersion(SemanticVersioningType.Maven,sdkInfo.SdkVersion)}"),
                            new XAttribute(k_IdentifierKey, sdkInfo.Identifier),
                            new XAttribute(k_VersionKey, sdkInfo.SdkVersion)
                        ),
                        new XComment(" Mediation SDK Android Adapters "),
                        adapters.OrderBy(info => info.Identifier)
                            .Select(info => new XElement("androidPackage",
                                new XAttribute("spec", $"{info.AndroidArtifact}:{VersionInfo.OptimisticVersion(SemanticVersioningType.Maven,sdkInfo.SdkVersion)}"),
                                new XAttribute(k_IdentifierKey, info.Identifier),
                                new XAttribute(k_VersionKey, info.InstalledVersion.Identifier)
                            ))
                    ),
                    new XElement("iosPods",
                        new XComment(" Mediation iOS SDK "),
                        new XElement("iosPod",
                            new XAttribute("name", sdkInfo.IosPod),
                            new XAttribute("version", VersionInfo.OptimisticVersion(SemanticVersioningType.CocoaPods, sdkInfo.SdkVersion)),
                            new XAttribute("source", k_IOSArtifactoryURL),
                            new XAttribute(k_IdentifierKey, sdkInfo.Identifier),
                            new XAttribute(k_VersionKey, sdkInfo.SdkVersion)
                        ),
                        new XComment(" Mediation Dependency Required For resolution "),
                        new XElement("iosPod",
                            new XAttribute("name", "Protobuf")
                        ),
                        new XComment(" Mediation SDK iOS Adapters "),
                        adapters.OrderBy(info => info.Identifier)
                            .Select(info => new XElement("iosPod",
                                new XAttribute("name", $"{info.IosPod}"),
                                new XAttribute("version", info.InstalledVersion.Version(SemanticVersioningType.CocoaPods)),
                                new XAttribute("source", k_IOSArtifactoryURL),
                                new XAttribute(k_IdentifierKey, info.Identifier),
                                new XAttribute(k_VersionKey, info.InstalledVersion.Identifier)
                            ))
                    )
                )
            ).Replace("&gt;", ">");
        }

        string FindLocalMavenRepo()
        {
            var searchFolders = new[] { "Packages/com.unity.services.mediation", "Packages/com.unity.accelerate", "Assets" };
            string repo = null;
            foreach (var folder in searchFolders)
            {
                try
                {
                    repo = Directory.GetDirectories(folder, "*", SearchOption.AllDirectories)
                        .FirstOrDefault(IsPathLikeLocalMavenPath);
                    if (repo != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (repo == null)
            {
                throw new FileNotFoundException($"Can't find local maven repository for Unity Mediation");
            }

            for (int i = 0; i < 4; i++)
            {
                repo = Path.GetDirectoryName(repo);
            }

            return repo;
        }

        internal bool IsPathLikeLocalMavenPath(string path)
        {
            if (m_MavenRegex == null)
            {
                m_MavenRegex = new Regex("Editor[\\\\/]Maven~[\\\\/]com[\\\\/]unity3d[\\\\/]mediation[\\\\/]mediation-sdk$");
            }

            return m_MavenRegex.IsMatch(path);
        }

        string FindPodSpecPath(string podName)
        {
            var searchFolders = new[] { "Packages/com.unity.services.mediation", "Packages/com.unity.accelerate", "Assets" };
            string podspec = null;
            foreach (var folder in searchFolders)
            {
                try
                {
                    podspec = Directory.GetFiles(folder, $"{podName}.podspec",
                        SearchOption.AllDirectories).FirstOrDefault();
                    if (podspec != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (podspec == null)
            {
                throw new FileNotFoundException($"Can't find {podName}.podspec");
            }

            return Path.GetDirectoryName(podspec)?.Replace("\\", "/");
        }

        /// <summary>
        /// Get a list of adapters currently in use with Mediation SDK
        /// </summary>
        /// <returns>Returns a List of AdapterInfo</returns>
        public virtual List<AdapterInfo> GetInstalledAdapters(bool generateXml = true)
        {
            var xmlPath = GetDependenciesPath();
            return GetInstalledAdapters(xmlPath, generateXml);
        }

        internal List<AdapterInfo> GetInstalledAdapters(string xmlPath, bool generateXml = true)
        {
            var needRegeneration = false;
            var sdkInfo = MediationSdkInfo.GetSdkInfo();
            var allAdapters = MediationSdkInfo.GetAllAdapters();
            var installedAdapters = new List<AdapterInfo>();
            var xmlDoc = XElement.Load(xmlPath);
            var xmlDeps = xmlDoc.Element("androidPackages")?.Elements("androidPackage");
            if (xmlDeps == null) return new List<AdapterInfo>();
            foreach (var element in xmlDeps)
            {
                var identifier = element.Attribute(k_IdentifierKey)?.Value;
                if (string.IsNullOrWhiteSpace(identifier))
                {
                    needRegeneration = true;
                    continue;
                }

                if (identifier.StartsWith(sdkInfo.Identifier))
                {
                    var sdkVersion = element.Attribute(k_VersionKey)?.Value;
                    if (sdkVersion != sdkInfo.SdkVersion)
                    {
                        needRegeneration = true;
                    }
                    //SDK component, not an adapter. Skip
                    continue;
                }

                var adapter = allAdapters.FirstOrDefault(info => info.Identifier == identifier);
                if (adapter == null)
                {
                    needRegeneration = true;
                    continue;
                }

                var versionIdentifier = element.Attribute(k_VersionKey)?.Value;
                if (!string.IsNullOrWhiteSpace(versionIdentifier) && Array.Exists(adapter.Versions, (x) => x.Identifier == versionIdentifier))
                {
                    adapter.InstalledVersion = adapter.Versions.First(versionInfo => versionInfo.Identifier == versionIdentifier);
                }
                else
                {
                    adapter.InstalledVersion = adapter.Versions.Last();
                    needRegeneration = true;
                }
                installedAdapters.Add(adapter);
            }

            if (needRegeneration && generateXml)
            {
                InstallOverrideAdapters(xmlPath, installedAdapters);
            }

            return installedAdapters;
        }

        /// <summary>
        /// Installs or updates an adapter to specified version.
        /// If the adapter is already installed under a different version, the version will be updated.
        /// If version is not specified, latest available version will be installed.
        /// </summary>
        /// <param name="identifier">The identifier of the adapter</param>
        /// <param name="versionInfo">Version of the adapter</param>
        /// <exception cref="InvalidOperationException">Thrown if combination of identifier/version is not valid</exception>
        public virtual void InstallAdapter(string identifier, VersionInfo versionInfo = null)
        {
            var xmlPath = GetDependenciesPath();

            InstallAdapter(identifier, versionInfo, xmlPath);
        }

        internal void InstallAdapter(string identifier, VersionInfo versionInfo, string xmlPath)
        {
            var adapter = MediationSdkInfo.GetAllAdapters().FirstOrDefault(info => info.Identifier == identifier);
            if (adapter == null)
            {
                throw new InvalidOperationException($"Can't install adapter with identifier \'{identifier}\'");
            }

            if (versionInfo != null && !Array.Exists(adapter.Versions,  (x) => x == versionInfo))
            {
                throw new InvalidOperationException(
                    $"Can't install adapter with identifier \'{identifier}\': version \'{versionInfo.Identifier}\' not available");
            }

            if (versionInfo == null)
            {
                versionInfo = adapter.Versions.First();
            }

            var installedAdapters = GetInstalledAdapters(xmlPath);
            var installedAdapter = installedAdapters.FirstOrDefault(info => info.Identifier == identifier);
            if (installedAdapter != null)
            {
                if (installedAdapter.InstalledVersion == versionInfo) return;
                installedAdapter.InstalledVersion = versionInfo;
            }
            else
            {
                adapter.InstalledVersion = versionInfo;
                installedAdapters.Add(adapter);
            }

            InstallOverrideAdapters(xmlPath, installedAdapters);
        }

        /// <summary>
        /// Remove an existing adapter from Mediation.
        /// If adapter is already uninstalled, no action will take place.
        /// </summary>
        /// <param name="identifier">The identifier of the adapter</param>
        /// <exception cref="InvalidOperationException">Thrown if identifier is not valid</exception>
        public virtual void UninstallAdapter(string identifier)
        {
            var xmlPath = GetDependenciesPath();
            UninstallAdapter(identifier, xmlPath);
        }

        internal void UninstallAdapter(string identifier, string xmlPath)
        {
            var installedAdapters = GetInstalledAdapters(xmlPath);
            var adapter = installedAdapters.FirstOrDefault(info => info.Identifier == identifier);
            if (adapter != null)
            {
                installedAdapters.Remove(adapter);
                InstallOverrideAdapters(xmlPath, installedAdapters);
            }
        }

        void InstallOverrideAdapters(string xmlPath, List<AdapterInfo> adapters)
        {
            File.WriteAllText(xmlPath, GenerateXmlContentWithAllDependencies(adapters));
            AdaptersChanged?.Invoke();
        }

        public void Apply(bool resolve = false)
        {
            AssetDatabase.ImportAsset(GetDependenciesPath());
#if UNITY_ANDROID
            if (resolve)
            {
                MobileDependencyResolverUtils.ResolveIfNeeded();
            }
#endif
        }

        string EnsureDependenciesFileInEditorFolder(string file)
        {
            if (Array.IndexOf(file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), "Editor") == -1)
            {
                var editorFolder = Path.Combine(Path.GetDirectoryName(file), "Editor");
                var newPath = Path.Combine(editorFolder, k_FileName);
                Debug.LogWarningFormat("{0} is not inside an Editor folder. Moving to: '{1}'", k_FileName, newPath);
                Directory.CreateDirectory(editorFolder);
                File.Move(file, newPath);
                var metaFile = file + ".meta";
                if (File.Exists(metaFile))
                {
                    var newMetaPath = Path.Combine(editorFolder, k_FileName) + ".meta";
                    File.Move(metaFile, newMetaPath);
                }
                AssetDatabase.Refresh();
                file = newPath;
            }

            return file;
        }
    }
}
