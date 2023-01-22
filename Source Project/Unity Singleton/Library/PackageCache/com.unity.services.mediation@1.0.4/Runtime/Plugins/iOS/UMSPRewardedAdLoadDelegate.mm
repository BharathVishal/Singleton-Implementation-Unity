#import "UMSPRewardedAdLoadDelegate.h"

@implementation UMSPRewardedAdLoadDelegate

- (id)initWithSuccessCallback:(LoadSuccessCallback)loadSuccess failCallback:(LoadFailCallback)loadFail {
    self = [super init];

    if (self) {
        self.loadSuccess = loadSuccess;
        self.loadFail = loadFail;
    }

    return self;
}

- (void)onRewardedFailedLoad:(UMSRewardedAd *)rewardedAd error:(UMSLoadError)error message:(NSString *)message {
    if (self.loadFail) {
        self.loadFail((__bridge void *)rewardedAd, (int)error, [message UTF8String]);
    }
}

- (void)onRewardedLoaded:(UMSRewardedAd *)rewardedAd {
    if (self.loadSuccess) {
        self.loadSuccess((__bridge void *)rewardedAd);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPRewardedAdLoadDelegateCreate(LoadSuccessCallback loadSuccessCallback, LoadFailCallback loadFailCallback) {
    UMSPRewardedAdLoadDelegate *delegate = [[UMSPRewardedAdLoadDelegate alloc]
                                            initWithSuccessCallback:loadSuccessCallback
                                                       failCallback:loadFailCallback];

    return (__bridge_retained void *)delegate;
}

void UMSPRewardedAdLoadDelegateDestroy(void *ptr) {
    if (!ptr) return;

    UMSPRewardedAdLoadDelegate *delegate = (__bridge_transfer UMSPRewardedAdLoadDelegate *)ptr;

    delegate.loadSuccess = nil;
    delegate.loadFail = nil;
}

#ifdef __cplusplus
}
#endif
