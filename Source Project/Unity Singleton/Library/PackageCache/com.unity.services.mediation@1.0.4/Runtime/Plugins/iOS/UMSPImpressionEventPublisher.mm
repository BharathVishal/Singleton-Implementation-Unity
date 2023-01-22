#import <UnityMediationSdk/UnityMediationSdk.h>
#import "UMSPImpressionDelegate.h"

#ifdef __cplusplus
extern "C" {
#endif

void UMSPImpressionEventPublisherSubscribe(void *ptr) {
    if (!ptr) return;

    UMSPImpressionDelegate *delegate = (__bridge UMSPImpressionDelegate *)ptr;

    [UMSImpressionEventPublisher subscribe:delegate];
}

void UMSPImpressionEventPublisherUnsubscribe(void *ptr) {
    if (!ptr) return;

    UMSPImpressionDelegate *delegate = (__bridge UMSPImpressionDelegate *)ptr;

    [UMSImpressionEventPublisher unsubscribe:delegate];
}

#ifdef __cplusplus
}
#endif
