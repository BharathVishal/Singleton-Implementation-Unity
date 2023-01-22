#if UNITY_ANDROID
using System.IO;
using UnityEditor.Android;

namespace Unity.Mediation.Build.Editor
{
    class AndroidProguardPostGenerateGradleProject : IPostGenerateGradleAndroidProject
    {
        const string k_ProguardFile = "proguard-unity.txt";
        const string k_ProguardMediationOption = "\r\n#Keep mediation adapters\r\n-keep class com.unity3d.mediation.** \r\n";

        public int callbackOrder { get; }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string proguardPath = Path.Combine(path, k_ProguardFile);

            File.AppendAllText(proguardPath, k_ProguardMediationOption);
        }
    }
}
#endif
