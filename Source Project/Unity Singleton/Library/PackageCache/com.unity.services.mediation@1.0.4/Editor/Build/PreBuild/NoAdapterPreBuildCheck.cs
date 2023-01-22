#if UNITY_ANDROID || UNITY_IOS
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.Services.Mediation.Adapters.Editor;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    class NoAdapterPreBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        public void OnPreprocessBuild(BuildReport report)
        {
            if (MediationSdkInfo.GetInstalledAdapters().Count == 0)
            {
                Debug.LogWarning("No Adapters Detected");
                if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
                {
                    //This check is currently not ran in batch mode
                    return;
                }
                bool terminateBuild = EditorUtility.DisplayDialog("No Adapters Detected", "Go to Project Settings -> Mediation to add an adapter\nDo you want to cancel the build?", "Cancel build", "Ignore");
                if (terminateBuild)
                {
                    throw new BuildFailedException("Build canceled");
                }
            }
        }
    }
}
#endif
