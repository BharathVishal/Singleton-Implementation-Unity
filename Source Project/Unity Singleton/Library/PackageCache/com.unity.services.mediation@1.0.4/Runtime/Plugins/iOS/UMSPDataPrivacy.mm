#import <UnityMediationSdk/UnityMediationSdk.h>

#ifdef __cplusplus
extern "C" {
#endif

void UMSPDataPrivacyUserGaveConsent(UMSConsentStatus consent, UMSDataPrivacyLaw law) {
    [UMSDataPrivacy userGaveConsent:consent toLaw:law];
}

UMSConsentStatus UMSPDataPrivacyGetConsentStatusForLaw(UMSDataPrivacyLaw law) {
    return [UMSDataPrivacy getConsentStatusForLaw:law];
}

#ifdef __cplusplus
}
#endif
