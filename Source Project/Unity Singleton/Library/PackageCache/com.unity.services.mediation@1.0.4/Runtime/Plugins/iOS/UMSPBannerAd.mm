#import <UnityMediationSdk/UnityMediationSdk.h>
#include <string.h>
#import "UnityAppController.h"
#import "UMSPBannerAdLoadDelegate.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef enum {
    TopCenter    = 0,
    TopLeft      = 1,
    TopRight     = 2,
    Center       = 3,
    MiddleLeft   = 4,
    MiddleRight  = 5,
    BottomCenter = 6,
    BottomLeft   = 7,
    BottomRight  = 8,
    None         = 9,
    Default      = 0
} UMSPBannerAdAnchor;


@interface UIView (RemoveConstraints)

- (void)removeAllConstraints;

@end


@implementation UIView (RemoveConstraints)

- (void)removeAllConstraints {
    UIView *superview = self.superview;

    for (NSLayoutConstraint *constraint in superview.constraints) {
        if (constraint.firstItem == self || constraint.secondItem == self) {
            [superview removeConstraint:constraint];
        }
    }
}

@end

void DisplayBannerAd(UMSBannerAdView *bannerAd, UIViewController *viewController,
                     UMSPBannerAdAnchor anchor, float offsetRatioX, float offsetRatioY) {
    if (bannerAd.superview) {
        [bannerAd removeAllConstraints];
    } else {
        [bannerAd removeFromSuperview];
        [viewController.view addSubview:bannerAd];
    }

    bannerAd.translatesAutoresizingMaskIntoConstraints = NO;

    NSLayoutAttribute attributeX = NSLayoutAttributeTop;
    NSLayoutAttribute attributeY = NSLayoutAttributeLeft;

    switch (anchor) {
        case TopCenter:
            attributeX = NSLayoutAttributeCenterX;
            attributeY = NSLayoutAttributeTop;
            break;

        case TopLeft:
            attributeX = NSLayoutAttributeLeft;
            attributeY = NSLayoutAttributeTop;
            break;

        case TopRight:
            attributeX = NSLayoutAttributeRight;
            attributeY = NSLayoutAttributeTop;
            break;

        case Center:
            attributeX = NSLayoutAttributeCenterX;
            attributeY = NSLayoutAttributeCenterY;
            break;

        case MiddleLeft:
            attributeX = NSLayoutAttributeLeft;
            attributeY = NSLayoutAttributeCenterY;
            break;

        case MiddleRight:
            attributeX = NSLayoutAttributeRight;
            attributeY = NSLayoutAttributeCenterY;
            break;

        case BottomCenter:
            attributeX = NSLayoutAttributeCenterX;
            attributeY = NSLayoutAttributeBottom;
            break;

        case BottomLeft:
        case None:
            attributeX = NSLayoutAttributeLeft;
            attributeY = NSLayoutAttributeBottom;
            break;

        case BottomRight:
            attributeX = NSLayoutAttributeRight;
            attributeY = NSLayoutAttributeBottom;
            break;
    }




    id constrainToItem = viewController.view;

    float offsetX = offsetRatioX * viewController.view.frame.size.width;
    float offsetY = offsetRatioY * viewController.view.frame.size.height;

    if (@available(iOS 11.0, *) && anchor != None) {
        constrainToItem = viewController.view.safeAreaLayoutGuide;
    }

    NSLayoutConstraint *verticalConstraint = [NSLayoutConstraint
                                              constraintWithItem:bannerAd
                                                       attribute:attributeY
                                                       relatedBy:NSLayoutRelationEqual
                                                          toItem:constrainToItem
                                                       attribute:attributeY
                                                      multiplier:1.0
                                                        constant:-offsetY];

    NSLayoutConstraint *horizontalConstraint = [NSLayoutConstraint
                                                constraintWithItem:bannerAd
                                                         attribute:attributeX
                                                         relatedBy:NSLayoutRelationEqual
                                                            toItem:constrainToItem
                                                         attribute:attributeX
                                                        multiplier:1.0
                                                          constant:offsetX];

    [NSLayoutConstraint activateConstraints:@[verticalConstraint, horizontalConstraint]];

    [viewController.view setNeedsLayout];
    [viewController.view layoutIfNeeded];
}

void * UMSPBannerAdCreate(const char *adUnitId, int width, int height) {
    NSString *adUnitIdString = [NSString stringWithUTF8String:adUnitId];
    UMSBannerAdViewSize *bannerAdSize = [UMSBannerAdViewSize bannerAdViewSizeFromCGSize:CGSizeMake(width, height)];

    UMSBannerAdView *bannerAd = [[UMSBannerAdView alloc]
                                 initWithAdUnitId:adUnitIdString
                                 bannerAdViewSize:bannerAdSize];

    NSLayoutConstraint *heightConstraint = [NSLayoutConstraint
                                            constraintWithItem:bannerAd
                                                     attribute:NSLayoutAttributeHeight
                                                     relatedBy:NSLayoutRelationEqual
                                                        toItem:nil
                                                     attribute:NSLayoutAttributeNotAnAttribute
                                                    multiplier:1.0
                                                      constant:bannerAd.bannerAdViewSize.size.height];

    NSLayoutConstraint *widthConstraint = [NSLayoutConstraint
                                           constraintWithItem:bannerAd
                                                    attribute:NSLayoutAttributeWidth
                                                    relatedBy:NSLayoutRelationEqual
                                                       toItem:nil
                                                    attribute:NSLayoutAttributeNotAnAttribute
                                                   multiplier:1.0
                                                     constant:bannerAd.bannerAdViewSize.size.width];

    [NSLayoutConstraint activateConstraints:@[heightConstraint, widthConstraint]];

    return (__bridge_retained void *)bannerAd;
}

void UMSPBannerAdLoad(void *bannerAdViewPtr, void *delegatePtr) {
    if (!bannerAdViewPtr) return;

    UMSBannerAdView *bannerAd = (__bridge UMSBannerAdView *)bannerAdViewPtr;

    UMSPBannerAdLoadDelegate *delegate = delegatePtr ? (__bridge UMSPBannerAdLoadDelegate *)delegatePtr : nil;

    bannerAd.delegate = delegate;

    UIViewController *viewController = [GetAppController() rootViewController];

    [bannerAd loadWithViewController:viewController];
}

void UMSPBannerAdDestroy(void *bannerAdViewPtr) {
    if (!bannerAdViewPtr) return;

    UMSBannerAdView *bannerAd = (__bridge_transfer UMSBannerAdView *)bannerAdViewPtr;
    bannerAd.delegate = nil;
    [bannerAd removeFromSuperview];
}

void UMSPBannerAdSetPosition(void *bannerAdViewPtr, int anchor, float offsetRatioX, float offsetRatioY) {
    if (!bannerAdViewPtr) return;

    UMSBannerAdView *bannerAd = (__bridge UMSBannerAdView *)bannerAdViewPtr;

    UIViewController *viewController = [GetAppController() rootViewController];

    dispatch_async(dispatch_get_main_queue(), ^{
        DisplayBannerAd(bannerAd, viewController, static_cast<UMSPBannerAdAnchor>(anchor), offsetRatioX, offsetRatioY);
    });
}

const char * UMSPBannerAdGetAdUnitId(void *bannerAdViewPtr) {
    if (!bannerAdViewPtr) return nil;

    UMSBannerAdView *bannerAd = (__bridge UMSBannerAdView *)bannerAdViewPtr;
    NSString *adUnitId = [[NSString alloc] initWithString:bannerAd.adUnitId];

    if (!adUnitId) return nil;

    return strdup([adUnitId UTF8String]);
}

int UMSPBannerAdGetAdState(void *bannerAdViewPtr) {
    if (!bannerAdViewPtr) return (int)UMSAdStateUnloaded;

    UMSBannerAdView *bannerAd = (__bridge UMSBannerAdView *)bannerAdViewPtr;

    return (int)[bannerAd getAdState];
}

#ifdef __cplusplus
}
#endif
