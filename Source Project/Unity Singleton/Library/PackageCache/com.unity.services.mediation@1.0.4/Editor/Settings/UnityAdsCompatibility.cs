#if UNITY_2019_1_OR_NEWER
using System.IO;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Mediation.Settings.Editor
{
    [InitializeOnLoad]
    class UnityAdsCompatibility
    {
        const string k_DialogTitle = "Advertisement (Legacy) package identified";
        const string k_DialogText  = "We detected that you have installed the Unity Ads package. " +
            "This package may conflict with Unity Mediation as it's content is included in Mediation, and this may cause build issues. " +
            "We recommend uninstalling the Unity Ads package when using Mediation.";
        const string k_DialogButtonUninstall = "Uninstall Unity Ads";
        const string k_DialogButtonCancel    = "Don't ask me again";

        const string k_DoNotShowAgainKey = "UninstallAdsDialogDontShow";

        const string k_UnityAdsPackageId = "com.unity.ads";

        static RemoveRequest s_PackageRemovalRequest;

        [InitializeOnLoadMethod]
        static void OnPackageLoad()
        {
            if (ShouldShowDialog())
            {
                ShowDialog();
            }
        }

        static void ShowDialog()
        {
            var requestUninstall = EditorUtility.DisplayDialog(
                k_DialogTitle,
                k_DialogText,
                k_DialogButtonUninstall,
                k_DialogButtonCancel);

            if (requestUninstall)
            {
                RemoveAdsPackage();
            }
            else
            {
                EditorPrefs.SetBool(k_DoNotShowAgainKey, true);
            }
        }

        static bool ShouldShowDialog()
        {
            bool requestedNotToShow = false;
            if (EditorPrefs.HasKey(k_DoNotShowAgainKey))
            {
                requestedNotToShow = EditorPrefs.GetBool(k_DoNotShowAgainKey);
            }

            bool packageInstalled = IsPackageInstalled(k_UnityAdsPackageId);

            return !requestedNotToShow && packageInstalled;
        }

        static bool IsPackageInstalled(string packageId)
        {
            bool packageFound = false;
            if (File.Exists("Packages/manifest.json"))
            {
                var jsonText = File.ReadAllText("Packages/manifest.json");
                packageFound = jsonText.Contains("\"" + packageId + "\"");
            }
            return packageFound;
        }

        static void RemoveAdsPackage()
        {
            s_PackageRemovalRequest = Client.Remove(k_UnityAdsPackageId);
            EditorApplication.update += OnEditorUpdate;
        }

        static void OnEditorUpdate()
        {
            if (s_PackageRemovalRequest.IsCompleted)
            {
                if (s_PackageRemovalRequest.Status == StatusCode.Success)
                {
                    Debug.Log("Removed: " + s_PackageRemovalRequest.PackageIdOrName);
                }
                else if (s_PackageRemovalRequest.Status >= StatusCode.Failure)
                {
                    Debug.Log(s_PackageRemovalRequest.Error.message);
                }
                EditorApplication.update -= OnEditorUpdate;
            }
        }
    }
}
#endif
