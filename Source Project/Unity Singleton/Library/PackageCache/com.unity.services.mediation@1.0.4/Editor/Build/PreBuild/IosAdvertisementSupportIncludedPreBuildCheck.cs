#if UNITY_IOS
using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.Services.Mediation.Build.Editor
{
    class IosAdvertisementSupportIncludedPreBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        const string k_MissingIosSupportPackageMessage = "The iOS Advertising Support package is missing from your solution, we highly recommend to install the iOS Advertising Support package by following the steps provided here: https://docs.unity.com/ads/InstallingTheiOS14SupportPackage.html";

        public void OnPreprocessBuild(BuildReport report)
        {
#if !HAVE_IOS_SUPPORT_PACKAGE
            Debug.LogError(k_MissingIosSupportPackageMessage);
#endif
        }
    }
}
#endif
