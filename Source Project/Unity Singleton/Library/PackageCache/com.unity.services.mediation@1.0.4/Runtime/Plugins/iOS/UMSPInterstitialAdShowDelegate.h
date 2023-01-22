#import <UnityMediationSdk/UnityMediationSdk.h>

typedef void (*StartedCallback)(void *interstitialAd);
typedef void (*ClickedCallback)(void *interstitialAd);
typedef void (*FinishedCallback)(void *interstitialAd);
typedef void (*FailedShowCallback)(void *interstitialAd, int error, const char *message);

@interface UMSPInterstitialAdShowDelegate : NSObject <UMSInterstitialAdShowDelegate>
@property (assign) StartedCallback started;
@property (assign) ClickedCallback clicked;
@property (assign) FinishedCallback finished;
@property (assign) FailedShowCallback failedShow;
- (id)initWithStartedCallback:(StartedCallback)started clickedCallback:(ClickedCallback)clicked finishedCallback:(FinishedCallback)finished failedShowCallback:(FailedShowCallback)failedShow;
@end
