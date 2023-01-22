#if UNITY_ANDROID
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    interface IAndroidInitializationListener
    {
        void onInitializationComplete();
        void onInitializationFailed(AndroidJavaObject errorCode, string msg);
    }
}
#endif
