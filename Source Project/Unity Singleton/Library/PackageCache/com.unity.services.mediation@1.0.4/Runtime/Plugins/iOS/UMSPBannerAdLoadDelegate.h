#import <UnityMediationSdk/UnityMediationSdk.h>

typedef void (*LoadSuccessCallback)(void *bannerAd);
typedef void (*LoadFailCallback)(void *bannerAd, int error, const char *message);
typedef void (*ClickedCallback)(void *bannerAd);
typedef void (*RefreshedCallback)(void *bannerAd, int error, const char *message);

@interface UMSPBannerAdLoadDelegate : NSObject <UMSBannerAdViewDelegate>
@property (assign) LoadSuccessCallback loadSuccess;
@property (assign) LoadFailCallback loadFail;
@property (assign) ClickedCallback clicked;
@property (assign) RefreshedCallback refreshed;
- (id)initWithSuccessCallback:(LoadSuccessCallback)loadSuccess failCallback:(LoadFailCallback)loadFail clickedCallback:(ClickedCallback)clicked refreshedCallback:(RefreshedCallback)refreshed;
@end
