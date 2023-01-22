using System;

namespace Unity.Services.Mediation.Adapters.Editor
{
    interface ISemanticVersioningFormatter
    {
        /// <summary>
        /// Returns optimistic version range
        /// <param name="version">The version being formatted</param>
        /// <returns>Returns a formatted version in the optimistic form</returns>
        /// </summary>
        string OptimisticVersion(string version);

        /// <summary>
        /// The latest version in the underlying format
        /// <returns>Returns the latest version identifier in the underlying format</returns>
        /// </summary>
        string LatestVersionIdentifier();
    }

    enum SemanticVersioningType
    {
        CocoaPods,
        Maven
    }

    static class SemanticVersioningFactory
    {
        public static ISemanticVersioningFormatter Formatter(SemanticVersioningType type)
        {
            switch (type)
            {
                case SemanticVersioningType.CocoaPods:
                    return new CocoapodsSemanticVersioningFormatter();
                case SemanticVersioningType.Maven:
                    return new MavenSemanticVersioningFormatter();
                default:
                    return new UnknownSemanticVersioningFormatter();
            }
        }
    }

    class CocoapodsSemanticVersioningFormatter : ISemanticVersioningFormatter
    {
        const string k_LatestVersionIdentifier = "";

        public string OptimisticVersion(string version)
        {
            return "~> " + version;
        }

        public string LatestVersionIdentifier()
        {
            return k_LatestVersionIdentifier;
        }
    }

    class MavenSemanticVersioningFormatter : ISemanticVersioningFormatter
    {
        const string k_LatestVersionIdentifier = "+";

        public string OptimisticVersion(string version)
        {
            var semanticVersion = Version.Parse(version);
            return $"[{semanticVersion.Major}.{semanticVersion.Minor},{semanticVersion.Major+1}.0[";
        }

        public string LatestVersionIdentifier()
        {
            return k_LatestVersionIdentifier;
        }
    }

    class UnknownSemanticVersioningFormatter : ISemanticVersioningFormatter
    {
        public string OptimisticVersion(string version)
        {
            throw new NotImplementedException();
        }

        public string LatestVersionIdentifier()
        {
            throw new NotImplementedException();
        }
    }
}
