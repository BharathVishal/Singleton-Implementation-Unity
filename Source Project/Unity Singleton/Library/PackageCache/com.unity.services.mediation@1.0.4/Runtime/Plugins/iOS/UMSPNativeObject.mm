#import <Foundation/Foundation.h>

#ifdef __cplusplus
extern "C" {
#endif

void UMSPBridgeTransfer(void *x) {
    if (!x) return;

    (__bridge_transfer id)x;
}

#ifdef __cplusplus
}
#endif
