namespace Unity.Services.Mediation
{
    /// <summary>
    /// DataPrivacy is an interface that is used to register the user's consent status.
    /// </summary>
    public interface IDataPrivacy
    {
        /// <summary>
        /// Notify the sdk if personal information can be legally collected.
        /// </summary>
        /// <param name="consent">The value should be ConsentStatus.NotDetermined by default,
        /// ConsentStatus.Given if the user accepted and is above the age of consent, in any other case,
        /// or ConsentStatus.Denied if the user refused or is not above the age of consent.</param>
        /// <param name="dataPrivacyLaw">Defines which law the consent status is applicable to</param>
        void UserGaveConsent(ConsentStatus consent, DataPrivacyLaw dataPrivacyLaw);

        /// <summary>
        /// Gets the consent status for a given data privacy law.
        /// </summary>
        /// <param name="dataPrivacyLaw">The data privacy law for which we want to know the consent status.</param>
        /// <returns>Returns the consent status for the given data privacy law.</returns>
        ConsentStatus GetConsentStatusForLaw(DataPrivacyLaw dataPrivacyLaw);
    }

    /// <summary>
    /// The list of currently supported data privacy laws.
    /// </summary>
    public enum DataPrivacyLaw
    {
        /// <summary>
        /// General Data Privacy Regulation, applicable to users residing in the EEA.
        /// </summary>
        GDPR = 0,

        /// <summary>
        /// California Consumer Privacy Act, applicable to users residing in California.
        /// </summary>
        CCPA = 1,

        /// <summary>
        /// Personal Information Protection Law, regarding ad personalization, applicable to users residing in China.
        /// </summary>
        PIPLAdPersonalization = 2,

        /// <summary>
        /// Personal Information Protection Law, regarding moving data out of China, applicable to users residing in China.
        /// </summary>
        PIPLDataTransport = 3
    }

    /// <summary>
    /// The list of possible consent status.
    /// </summary>
    public enum ConsentStatus
    {
        /// <summary>
        /// The consent has neither been given nor denied yet.
        /// </summary>
        NotDetermined = 0,

        /// <summary>
        /// The consent has been explicitly given.
        /// </summary>
        Given = 1,

        /// <summary>
        /// The consent has been explicitly denied.
        /// </summary>
        Denied = 2
    }
}
