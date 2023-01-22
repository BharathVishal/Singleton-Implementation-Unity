#import "UMSPBannerAdLoadDelegate.h"

@implementation UMSPBannerAdLoadDelegate

- (id)initWithSuccessCallback:(LoadSuccessCallback)loadSuccess failCallback:(LoadFailCallback)loadFail clickedCallback:(ClickedCallback)clicked refreshedCallback:(RefreshedCallback)refreshed {
    self = [super init];

    if (self) {
        self.loadSuccess = loadSuccess;
        self.loadFail = loadFail;
        self.clicked = clicked;
        self.refreshed = refreshed;
    }

    return self;
}

- (void)onBannerAdViewFailedLoad:(UMSBannerAdView *)bannerAd error:(UMSLoadError)error message:(NSString *)message {
    if (self.loadFail) {
        self.loadFail((__bridge void *)bannerAd, (int)error, [message UTF8String]);
    }
}

- (void)onBannerAdViewLoaded:(UMSBannerAdView *)bannerAd {
    if (self.loadSuccess) {
        self.loadSuccess((__bridge void *)bannerAd);
    }
}

- (void)onBannerAdViewClicked:(UMSBannerAdView *)bannerAd {
    if (self.clicked) {
        self.clicked((__bridge void *)bannerAd);
    }
}

- (void)onBannerAdViewRefreshed:(UMSBannerAdView *_Nullable)bannerAd error:(NSError *)error {
    if (self.refreshed) {
        if (error) {
            self.refreshed((__bridge void *)bannerAd, (int)error.code, [error.description UTF8String]);
        } else {
            self.refreshed((__bridge void *)bannerAd, (int)-1, nil);
        }
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPBannerAdLoadDelegateCreate(LoadSuccessCallback loadSuccessCallback, LoadFailCallback loadFailCallback, ClickedCallback clickedCallback, RefreshedCallback refreshedCallback) {
    UMSPBannerAdLoadDelegate *delegate = [[UMSPBannerAdLoadDelegate alloc]
                                          initWithSuccessCallback:loadSuccessCallback
                                                     failCallback:loadFailCallback
                                                  clickedCallback:clickedCallback
                                                refreshedCallback:refreshedCallback];

    return (__bridge_retained void *)delegate;
}

void UMSPBannerAdLoadDelegateDestroy(void *ptr) {
    if (!ptr) return;

    UMSPBannerAdLoadDelegate *delegate = (__bridge_transfer UMSPBannerAdLoadDelegate *)ptr;

    delegate.loadSuccess = nil;
    delegate.loadFail = nil;
    delegate.clicked = nil;
    delegate.refreshed = nil;
}

#ifdef __cplusplus
}
#endif
