#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidMediationService : IPlatformMediationService, IDisposable, IAndroidInitializationListener
    {
        AndroidJavaClass m_UnityMediationClass;
        AndroidJavaClass m_InitializationConfiguration;
        AndroidInitializationListener m_InitializationListener;
        volatile bool m_Disposed;

        public AndroidMediationService()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_UnityMediationClass         = new AndroidJavaClass("com.unity3d.mediation.UnityMediation");
                    m_InitializationConfiguration = new AndroidJavaClass("com.unity3d.mediation.InitializationConfiguration");
                    m_InitializationListener      = new AndroidInitializationListener(this);

                }
                catch (Exception e)
                {
                    Debug.LogError("Error while loading Mediation SDK. Mediation SDK will not initialize. " +
                        "Please check your build settings, and make sure Mediation SDK is integrated properly.");
                    Debug.LogException(e);
                }
            });
        }

        void Dispose(bool disposing)
        {
            if (m_Disposed) return;
            m_Disposed = true;
            if (disposing)
            {
                //AndroidJavaObjects are created and destroyed with JNI's NewGlobalRef and DeleteGlobalRef,
                //Therefore must be used on the same attached thread. In this case, it's Unity thread.
                ThreadUtil.Post(state =>
                {
                    m_UnityMediationClass?.Dispose();
                    m_UnityMediationClass = null;
                    m_InitializationListener = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidMediationService()
        {
            Dispose(false);
        }

        bool CheckDisposedAndLogError(string message)
        {
            if (!m_Disposed) return false;
            Debug.LogErrorFormat("Unity Mediation SDK: {0}: Instance of type {1} is disposed. Please create a new instance in order to call any method.", message, GetType().FullName);
            return true;
        }

        public event EventHandler OnInitializationComplete;
        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;

        public InitializationState InitializationState
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot call InitializationState")) return InitializationState.Uninitialized;
                try
                {
                    return m_UnityMediationClass
                        .CallStatic<AndroidJavaObject>("getInitializationState")
                        .ToEnum<InitializationState>();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return InitializationState.Uninitialized;
                }
            }
        }

        public string SdkVersion
        {
            get
            {
                if (CheckDisposedAndLogError("Cannot retrieve Sdk Version")) return String.Empty;
                try
                {
                    return m_UnityMediationClass
                        .CallStatic<string>("getSdkVersion");
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return String.Empty;
                }
            }
        }

        public void Initialize(string gameId, string installId)
        {
            if (CheckDisposedAndLogError("Cannot call Initialize()")) return;
            ThreadUtil.Post(state =>
            {
                try
                {
                    AndroidJavaObject config = m_InitializationConfiguration.CallStatic<AndroidJavaObject>("builder")
                        .Call<AndroidJavaObject>("setGameId", gameId)
                        .Call<AndroidJavaObject>("setInitializationListener", m_InitializationListener)
                        .Call<AndroidJavaObject>("setOption", "installation_id", installId)
                        .Call<AndroidJavaObject>("build");

                    m_UnityMediationClass.CallStatic("initialize", config);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    var args = new InitializationErrorEventArgs(SdkInitializationError.Unknown, "Failed to initialize native SDK - " + e.Message);
                    OnInitializationFailed?.Invoke(null, args);
                }
            });
        }


        public void onInitializationComplete()
        {
            OnInitializationComplete?.Invoke(null, EventArgs.Empty);
        }

        public void onInitializationFailed(AndroidJavaObject error, string msg)
        {
            SdkInitializationError sdkError = error.ToEnum<SdkInitializationError>();
            OnInitializationFailed?.Invoke(null, new InitializationErrorEventArgs(sdkError, msg));
        }
    }
}
#endif
