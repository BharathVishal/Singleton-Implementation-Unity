#import "UMSPImpressionDelegate.h"

@implementation UMSPImpressionDelegate

- (id)initWithCallback:(ImpressionCallback)onImpression {
    self = [super init];

    if (self) {
        self.onImpression = onImpression;
    }

    return self;
}

- (void)onImpressionWithAdUnitId:(NSString *)adUnitId impressionData:(UMSImpressionData *)impressionData {
    if (!self.onImpression) {
        return;
    }

    UMSPImpressionData unityImpressionData;

    if (impressionData) {
        unityImpressionData.Timestamp = [impressionData.timestamp UTF8String];
        unityImpressionData.AdUnitName = [impressionData.adUnitName UTF8String];
        unityImpressionData.AdUnitId = [impressionData.adUnitId UTF8String];
        unityImpressionData.AdUnitFormat = [impressionData.adUnitFormat UTF8String];
        unityImpressionData.ImpressionId = [impressionData.impressionId UTF8String];
        unityImpressionData.Currency = [impressionData.currency UTF8String];
        unityImpressionData.RevenueAccuracy = [impressionData.revenueAccuracy UTF8String];
        unityImpressionData.publisherRevenuePerImpression = [impressionData.publisherRevenuePerImpression doubleValue];
        unityImpressionData.publisherRevenuePerImpressionInMicros = impressionData.publisherRevenuePerImpressionInMicros;
        unityImpressionData.AdSourceName = [impressionData.adSourceName UTF8String];
        unityImpressionData.AdSourceInstance = [impressionData.adSourceInstance UTF8String];
        unityImpressionData.AppVersion = [impressionData.appVersion UTF8String];
        unityImpressionData.LineItemId = [impressionData.lineItemId UTF8String];
        unityImpressionData.LineItemName = [impressionData.lineItemName UTF8String];
        unityImpressionData.LineItemPriority = [impressionData.lineItemPriority UTF8String];
        unityImpressionData.Country = [impressionData.country UTF8String];
    }

    self.onImpression([adUnitId UTF8String], impressionData ? &unityImpressionData : nil);
}

@end

#ifdef __cplusplus
extern "C" {
#endif

void * UMSPImpressionDelegateCreate(ImpressionCallback impressionCallback) {
    UMSPImpressionDelegate *delegate = [[UMSPImpressionDelegate alloc] initWithCallback:impressionCallback];

    return (__bridge_retained void *)delegate;
}

void UMSPImpressionDelegateDestroy(void *ptr) {
    if (!ptr) return;

    UMSPImpressionDelegate *delegate = (__bridge_transfer UMSPImpressionDelegate *)ptr;

    delegate.onImpression = nil;
}

#ifdef __cplusplus
}

#endif
