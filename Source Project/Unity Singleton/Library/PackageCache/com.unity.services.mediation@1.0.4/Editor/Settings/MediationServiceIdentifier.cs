using Unity.Services.Core.Editor;

namespace Unity.Services.Mediation.Settings.Editor
{
    /// <summary>
    /// Implementation of the <see cref="IEditorGameServiceIdentifier"/> for the Mediation package.
    /// </summary>
    /// <remarks>This identifier MUST be a public struct.</remarks>
    public struct MediationServiceIdentifier : IEditorGameServiceIdentifier
    {
        internal const string k_PackageDisplayName = "Unity Mediation";

        /// <summary>
        /// Key for the Mediation package
        /// </summary>
        /// <returns>Returns the key associated to the Mediation package.</returns>
        public string GetKey()
        {
            return k_PackageDisplayName;
        }
    }
}
