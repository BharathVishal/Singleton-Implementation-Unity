#import <UnityMediationSdk/UnityMediationSdk.h>

typedef void (*StartedCallback)(void *rewardedAd);
typedef void (*ClickedCallback)(void *rewardedAd);
typedef void (*FinishedCallback)(void *rewardedAd);
typedef void (*FailedShowCallback)(void *rewardedAd, int error, const char *message);
typedef void (*UserRewardedCallback)(void *rewardedAd, const char *type, const char *amount);

@interface UMSPRewardedAdShowDelegate : NSObject <UMSRewardedAdShowDelegate>
@property (assign) StartedCallback started;
@property (assign) ClickedCallback clicked;
@property (assign) FinishedCallback finished;
@property (assign) FailedShowCallback failedShow;
@property (assign) UserRewardedCallback userRewarded;
- (id)initWithStartedCallback:(StartedCallback)started clickedCallback:(ClickedCallback)clicked finishedCallback:(FinishedCallback)finished failedShowCallback:(FailedShowCallback)failedShow userRewardedCallback:(UserRewardedCallback)userRewarded;
@end
