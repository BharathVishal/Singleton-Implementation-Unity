#if UNITY_IOS
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation.Platform
{
    class IosDataPrivacy : IDataPrivacy
    {
        static IosDataPrivacy s_Instance;

        public IosDataPrivacy()
        {
            s_Instance = this;
        }

        public void Dispose()
        {
            if (this == s_Instance)
            {
                s_Instance = null;
            }
        }

        ~IosDataPrivacy()
        {
            Dispose();
        }

        public void UserGaveConsent(ConsentStatus consent, DataPrivacyLaw dataPrivacyLaw)
        {
            UnityDataPrivacyUserGaveConsent(consent, dataPrivacyLaw);
        }

        public ConsentStatus GetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw)
        {
            return UnityDataPrivacyGetConsentStatusForLaw(dataPrivacyLaw);
        }

        [DllImport("__Internal", EntryPoint = "UMSPDataPrivacyUserGaveConsent")]
        static extern void UnityDataPrivacyUserGaveConsent(ConsentStatus consent, DataPrivacyLaw law);

        [DllImport("__Internal", EntryPoint = "UMSPDataPrivacyGetConsentStatusForLaw")]
        static extern ConsentStatus UnityDataPrivacyGetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw);
    }
}
#endif
