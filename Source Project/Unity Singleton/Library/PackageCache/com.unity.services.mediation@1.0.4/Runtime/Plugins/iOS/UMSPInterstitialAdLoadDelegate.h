#import <UnityMediationSdk/UnityMediationSdk.h>

typedef void (*LoadSuccessCallback)(void *interstitialAd);
typedef void (*LoadFailCallback)(void *interstitialAd, int error, const char *message);

@interface UMSPInterstitialAdLoadDelegate : NSObject <UMSInterstitialAdLoadDelegate>
@property (assign) LoadSuccessCallback loadSuccess;
@property (assign) LoadFailCallback loadFail;
- (id)initWithSuccessCallback:(LoadSuccessCallback)loadSuccess failCallback:(LoadFailCallback)loadFail;
@end
