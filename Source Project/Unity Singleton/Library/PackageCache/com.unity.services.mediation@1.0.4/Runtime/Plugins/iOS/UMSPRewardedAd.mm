#import "UnityAppController.h"
#import <UnityMediationSdk/UnityMediationSdk.h>
#import <UnityMediationSdk/UMSRewardedAdShowOptions.h>
#import "UMSPRewardedAdLoadDelegate.h"
#import "UMSPRewardedAdShowDelegate.h"
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif

struct S2SRedeemData {
    const char *userId;
    const char *customData;
};

void * UMSPRewardedAdCreate(const char *adUnitId) {
    NSString *adUnitIdString = [NSString stringWithUTF8String:adUnitId];
    UMSRewardedAd *rewardedAd = [[UMSRewardedAd alloc]
                                 initWithAdUnitId:adUnitIdString];

    return (__bridge_retained void *)rewardedAd;
}

void UMSPRewardedAdLoad(void *ptr, void *delegatePtr) {
    if (!ptr) return;

    UMSRewardedAd *rewardedAd = (__bridge UMSRewardedAd *)ptr;
    UMSPRewardedAdLoadDelegate *delegate = delegatePtr ? (__bridge UMSPRewardedAdLoadDelegate *)delegatePtr : nil;

    [rewardedAd loadWithDelegate:delegate];
}

void UMSPRewardedAdShow(void *ptr, void *delegatePtr, S2SRedeemData s2sRedeemData) {
    if (!ptr) return;

    UMSRewardedAd *rewardedAd = (__bridge UMSRewardedAd *)ptr;
    UMSPRewardedAdShowDelegate *delegate = delegatePtr ? (__bridge UMSPRewardedAdShowDelegate *)delegatePtr : nil;

    UMSRewardedAdShowOptions *showOptions = [[UMSRewardedAdShowOptions alloc] init];

    if (s2sRedeemData.userId != NULL) {
        showOptions.publisherData.userId = [NSString stringWithUTF8String:s2sRedeemData.userId];
    }

    if (s2sRedeemData.customData != NULL) {
        showOptions.publisherData.customData = [NSString stringWithUTF8String:s2sRedeemData.customData];
    }

    UIViewController *viewController = [GetAppController() rootViewController];

    [rewardedAd showWithViewController:viewController
                              delegate:delegate
                           showOptions:showOptions];
}

const char * UMSPRewardedAdGetAdUnitId(void *ptr) {
    if (!ptr) return nil;

    UMSRewardedAd *rewardedAd = (__bridge UMSRewardedAd *)ptr;
    NSString *adUnitId = [rewardedAd getAdUnitId];

    if (!adUnitId) return nil;

    return strdup([adUnitId UTF8String]);
}

int UMSPRewardedAdGetAdState(void *ptr) {
    if (!ptr) return (int)UMSAdStateUnloaded;

    UMSRewardedAd *rewardedAd = (__bridge UMSRewardedAd *)ptr;

    return (int)[rewardedAd getAdState];
}

#ifdef __cplusplus
}
#endif
