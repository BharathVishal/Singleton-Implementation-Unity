#import "UnityAppController.h"
#import <UnityMediationSdk/UnityMediationSdk.h>
#import "UMSPInterstitialAdLoadDelegate.h"
#import "UMSPInterstitialAdShowDelegate.h"
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPInterstitialAdCreate(const char *adUnitId) {
    NSString *adUnitIdString = [NSString stringWithUTF8String:adUnitId];
    UMSInterstitialAd *interstitialAd = [[UMSInterstitialAd alloc]
                                         initWithAdUnitId:adUnitIdString];

    return (__bridge_retained void *)interstitialAd;
}

void UMSPInterstitialAdLoad(void *ptr, void *delegatePtr) {
    if (!ptr) return;

    UMSInterstitialAd *interstitialAd = (__bridge UMSInterstitialAd *)ptr;
    UMSPInterstitialAdLoadDelegate *delegate = delegatePtr ? (__bridge UMSPInterstitialAdLoadDelegate *)delegatePtr : nil;

    [interstitialAd loadWithDelegate:delegate];
}

void UMSPInterstitialAdShow(void *ptr, void *delegatePtr) {
    if (!ptr) return;

    UMSInterstitialAd *interstitialAd = (__bridge UMSInterstitialAd *)ptr;
    UMSPInterstitialAdShowDelegate *delegate = delegatePtr ? (__bridge UMSPInterstitialAdShowDelegate *)delegatePtr : nil;

    UIViewController *viewController = [GetAppController() rootViewController];

    [interstitialAd showWithViewController:viewController
                                  delegate:delegate];
}

const char * UMSPInterstitialAdGetAdUnitId(void *ptr) {
    if (!ptr) return nil;

    UMSInterstitialAd *interstitialAd = (__bridge UMSInterstitialAd *)ptr;
    NSString *adUnitId = [interstitialAd getAdUnitId];

    if (!adUnitId) return nil;

    return strdup([adUnitId UTF8String]);
}

int UMSPInterstitialAdGetAdState(void *ptr) {
    if (!ptr) return (int)UMSAdStateUnloaded;

    UMSInterstitialAd *interstitialAd = (__bridge UMSInterstitialAd *)ptr;

    return (int)[interstitialAd getAdState];
}

#ifdef __cplusplus
}
#endif
