#import "UMSPInterstitialAdLoadDelegate.h"

@implementation UMSPInterstitialAdLoadDelegate

- (id)initWithSuccessCallback:(LoadSuccessCallback)loadSuccess failCallback:(LoadFailCallback)loadFail {
    self = [super init];

    if (self) {
        self.loadSuccess = loadSuccess;
        self.loadFail = loadFail;
    }

    return self;
}

- (void)onInterstitialFailedLoad:(UMSInterstitialAd *)interstitialAd error:(UMSLoadError)error message:(NSString *)message {
    if (self.loadFail) {
        self.loadFail((__bridge void *)interstitialAd, (int)error, [message UTF8String]);
    }
}

- (void)onInterstitialLoaded:(UMSInterstitialAd *)interstitialAd {
    if (self.loadSuccess) {
        self.loadSuccess((__bridge void *)interstitialAd);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPInterstitialAdLoadDelegateCreate(LoadSuccessCallback loadSuccessCallback, LoadFailCallback loadFailCallback) {
    UMSPInterstitialAdLoadDelegate *delegate = [[UMSPInterstitialAdLoadDelegate alloc]
                                                initWithSuccessCallback:loadSuccessCallback
                                                           failCallback:loadFailCallback];

    return (__bridge_retained void *)delegate;
}

void UMSPInterstitialAdLoadDelegateDestroy(void *ptr) {
    if (!ptr) return;

    UMSPInterstitialAdLoadDelegate *delegate = (__bridge_transfer UMSPInterstitialAdLoadDelegate *)ptr;

    delegate.loadSuccess = nil;
    delegate.loadFail = nil;
}

#ifdef __cplusplus
}
#endif
