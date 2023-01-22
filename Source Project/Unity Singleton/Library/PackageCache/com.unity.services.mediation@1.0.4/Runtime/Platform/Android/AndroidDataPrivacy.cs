#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class AndroidDataPrivacy : IDataPrivacy, IDisposable
    {
        AndroidJavaClass m_DataPrivacyClass;
        volatile bool m_Disposed;

        public AndroidDataPrivacy()
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    m_DataPrivacyClass = new AndroidJavaClass("com.unity3d.mediation.DataPrivacy");
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while loading Mediation Data Privacy SDK. Mediation Data Privacy SDK will not initialize. " +
                        "Please check your build settings, and make sure Mediation Data Privacy SDK is integrated properly.");
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
                    m_DataPrivacyClass?.Dispose();
                    m_DataPrivacyClass = null;
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AndroidDataPrivacy()
        {
            Dispose(false);
        }

        public void UserGaveConsent(ConsentStatus consent, DataPrivacyLaw dataPrivacyLaw)
        {
            ThreadUtil.Send(state =>
            {
                try
                {
                    using (var activity = ActivityUtil.GetUnityActivity())
                    {
                        AndroidJavaObject consentJava = AndroidJavaObjectExtensions.ToAndroidEnum("com.unity3d.mediation.ConsentStatus", (int)consent);
                        AndroidJavaObject lawJava = AndroidJavaObjectExtensions.ToAndroidEnum("com.unity3d.mediation.DataPrivacyLaw", (int)dataPrivacyLaw);

                        m_DataPrivacyClass.CallStatic("userGaveConsent", consentJava, lawJava, activity);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Error while submitting consent status.");
                    Debug.LogException(e);
                }
            });
        }

        public ConsentStatus GetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw)
        {
            try
            {
                using (var activity = ActivityUtil.GetUnityActivity())
                {
                    AndroidJavaObject lawJava =  AndroidJavaObjectExtensions.ToAndroidEnum("com.unity3d.mediation.DataPrivacyLaw", (int)dataPrivacyLaw);
                    return m_DataPrivacyClass.CallStatic<AndroidJavaObject>(
                        "getConsentStatusForLaw",
                        lawJava,
                        activity).ToEnum<ConsentStatus>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error while retrieving consent status.");
                Debug.LogException(e);
                return ConsentStatus.NotDetermined;
            }
        }
    }
}
#endif
