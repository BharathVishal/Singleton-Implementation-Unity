using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Unity.Services.Mediation.Adapters.Editor
{
    [Serializable]
    class MediationInfo
    {
        public MediationInfo()
        {
            SdkInfo = new SdkInfo();
            Adapters = new AdapterInfo[0];
        }

        public SdkInfo SdkInfo;
        public AdapterInfo[] Adapters;

        public override string ToString()
        {
            return $"MediationInfo: {EditorJsonUtility.ToJson(this)}";
        }
    }

    [Serializable]
    class SdkInfo
    {
        /// <summary>
        /// SDK Identifier
        /// </summary>
        public string Identifier;

        /// <summary>
        /// SDK Display Name
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// SDK Android artifact
        /// </summary>
        public string AndroidArtifact;

        /// <summary>
        /// SDK iOS Pod
        /// </summary>
        public string IosPod;

        /// <summary>
        /// Package version
        /// </summary>
        public string PackageVersion;

        /// <summary>
        /// Sdk version
        /// </summary>
        public string SdkVersion;
    }

    /// <summary>
    /// Information for a single Mediation Adapter
    /// </summary>
    [Serializable]
    class AdapterInfo
    {
        AdapterInfo()
        {
        }

        /// <summary>
        /// Identifier for this Adapter
        /// </summary>
        public string Identifier;

        /// <summary>
        /// Identifier for this Adapter's network on the unity dashboard
        /// </summary>
        public string DashboardId;

        /// <summary>
        /// Adapter Display Name
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Adapter Android artifact
        /// </summary>
        public string AndroidArtifact;

        /// <summary>
        /// Adapter iOS Pod
        /// </summary>
        public string IosPod;

        /// <summary>
        /// Installed Version for this Adapter
        /// </summary>
        public VersionInfo InstalledVersion;

        /// <summary>
        /// Versions Available for this Adapter
        /// </summary>
        public VersionInfo[] Versions;

        /// <summary>
        /// Repositories required for this Adapter
        /// </summary>
        public string[] Repositories;

        /// <summary>
        /// Returns Adapter Info
        /// </summary>
        /// <returns>string with adapter info</returns>
        public override string ToString()
        {
            return $"AdapterInfo: {EditorJsonUtility.ToJson(this)}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AdapterInfo other)) return false;
            return Equals(other);
        }

        protected bool Equals(AdapterInfo other)
        {
            return Identifier == other.Identifier &&
                DashboardId == other.DashboardId &&
                DisplayName == other.DisplayName &&
                AndroidArtifact == other.AndroidArtifact &&
                IosPod == other.IosPod &&
                Equals(InstalledVersion, other.InstalledVersion) &&
                ArrayUtility.ArrayEquals(Versions, other.Versions) &&
                ArrayUtility.ArrayEquals(Repositories, other.Repositories);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Identifier != null ? Identifier.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DashboardId != null ? DashboardId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (DisplayName != null ? DisplayName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (AndroidArtifact != null ? AndroidArtifact.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (IosPod != null ? IosPod.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (InstalledVersion != null ? InstalledVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Versions != null ? Versions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Repositories != null ? Repositories.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    /// <summary>
    /// Information for a single Mediation Adapter Version
    /// </summary>
    [Serializable]
    class VersionInfo
    {
        /// <summary>
        /// Default Identifier value for an adapter set to latest supported
        /// </summary>
        internal const string k_Latest = "latest";

        /// <summary>
        /// Identifier for the version ie: 0.0.1
        /// </summary>
        public string Identifier;

        /// <summary>
        /// Value to display to the user
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Returns optimistic version range for a given semantic type
        /// <param name="type">The format type</param>
        /// <param name="version">The version being formatted</param>
        /// <returns>Returns a formatted version in the optimistic form</returns>
        /// </summary>
        public static string OptimisticVersion(SemanticVersioningType type, string version)
        {
            var semanticVersioningFormatter = SemanticVersioningFactory.Formatter(type);
            return semanticVersioningFormatter.OptimisticVersion(version);
        }

        /// <summary>
        /// Returns true is the Identifier is set to latest supported
        /// </summary>
        public bool IsLatestSupported()
        {
            return Identifier == k_Latest;
        }

        /// <summary>
        /// Returns the presentation of the version required by the resolver xml for the given semantic versioning type
        /// <param name="type">The type of format</param>
        /// <returns>Returns the version adhering to the given format</returns>
        /// </summary>
        public string Version(SemanticVersioningType type)
        {
            var semanticVersioningFormatter = SemanticVersioningFactory.Formatter(type);
            return IsLatestSupported() ? semanticVersioningFormatter.LatestVersionIdentifier() : Identifier;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is VersionInfo other)) return false;
            return Equals(other);
        }

        protected bool Equals(VersionInfo other)
        {
            return Identifier == other.Identifier;
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }

    /// <summary>
    /// Holds and Manages Adapter Information for Unity Mediation
    /// </summary>
    static class MediationSdkInfo
    {
        const string k_InfoFile = "MediationSdkInfo.json";

        internal static MediationInfo s_MediationInfo;

        internal static AdaptersDependencyGenerator s_Generator = new AdaptersDependencyGenerator();

        public static event Action AdaptersChanged
        {
            add => s_Generator.AdaptersChanged += value;
            remove => s_Generator.AdaptersChanged -= value;
        }

        static void LoadSdkInfoIfNeeded()
        {
            if (s_MediationInfo == null)
            {
                var infoPath = FindMediationSdkInfoPath();
                if (infoPath == null)
                {
                    throw new FileNotFoundException($"Can't find {k_InfoFile}");
                }

                var infoJson = File.ReadAllText(infoPath);
                s_MediationInfo = new MediationInfo();
                EditorJsonUtility.FromJsonOverwrite(infoJson, s_MediationInfo);
            }
        }

        internal static string FindMediationSdkInfoPath()
        {
            // Search for file in these folders.
            // TODO: search only in mediation after release
            var searchFolders = new[] { "Packages/com.unity.services.mediation", "Packages/com.unity.accelerate", "Assets" };
            string sdkInfoPath = null;
            foreach (var folder in searchFolders)
            {
                try
                {
                    sdkInfoPath = Directory.GetFiles(folder, k_InfoFile,
                        SearchOption.AllDirectories).FirstOrDefault();
                    if (sdkInfoPath != null)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return sdkInfoPath;
        }

        public static SdkInfo GetSdkInfo()
        {
            LoadSdkInfoIfNeeded();
            return s_MediationInfo.SdkInfo;
        }

        /// <summary>
        /// Get a list of all available adapters compatible with the current Mediation SDK
        /// </summary>
        /// <returns>Returns a List of AdapterInfo</returns>
        public static List<AdapterInfo> GetAllAdapters()
        {
            LoadSdkInfoIfNeeded();
            return new List<AdapterInfo>(s_MediationInfo.Adapters);
        }

        /// <summary>
        /// Get a list of adapters currently in use with Mediation SDK
        /// </summary>
        /// <returns>Returns a List of AdapterInfo</returns>
        public static List<AdapterInfo> GetInstalledAdapters(bool generateXml = true)
        {
            return s_Generator.GetInstalledAdapters(generateXml);
        }

        /// <summary>
        /// Installs or updates an adapter to specified version.
        /// If the adapter is already installed under a different version, the version will be updated.
        /// If version is not specified, latest available version will be installed.
        /// </summary>
        /// <param name="identifier">The identifier of the adapter</param>
        /// <param name="versionInfo">Version of the adapter</param>
        /// <exception cref="InvalidOperationException">Thrown if combination of identifier/version is not valid</exception>
        public static void Install(string identifier, VersionInfo versionInfo = null)
        {
            s_Generator.InstallAdapter(identifier, versionInfo);
        }

        /// <summary>
        /// Remove an existing adapter from Mediation.
        /// If adapter is already uninstalled, no action will take place.
        /// </summary>
        /// <param name="identifier">The identifier of the adapter</param>
        /// <exception cref="InvalidOperationException">Thrown if identifier is not valid</exception>
        public static void Uninstall(string identifier)
        {
            s_Generator.UninstallAdapter(identifier);
        }

        /// <summary>
        /// Apply changes.
        /// Will cause the xml dependency file to be imported.
        /// If resolve is set to true, dependency resolution will be triggered.
        /// </summary>
        public static void Apply(bool resolve = false)
        {
            s_Generator.Apply(resolve);
        }
    }
}
