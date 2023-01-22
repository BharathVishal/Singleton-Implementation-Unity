#import <UnityMediationSdk/UnityMediationSdk.h>
#import <UnityMediationSdk/UMSInitializationConfiguration.h>

typedef void (*InitSuccessCallback)();
typedef void (*InitFailureCallback)(int, const char *);

static InitSuccessCallback s_InitializationSuccess;
static InitFailureCallback s_InitializationFailed;

@interface UMSPInitializeDelegate : NSObject <UMSInitializationDelegate>
@end

@implementation UMSPInitializeDelegate

- (void)onInitializationComplete {
    if (s_InitializationSuccess) {
        s_InitializationSuccess();
    }
}

- (void)onInitializationFailed:(UMSSdkInitializationError)errorCode message:(NSString *)message {
    if (s_InitializationFailed) {
        s_InitializationFailed((int)errorCode, [message UTF8String]);
    }
}

@end

#ifdef __cplusplus
extern "C" {
#endif

int UMSPUnityMediationGetInitializationState() {
    return (int)[UMSUnityMediation getInitializationState];
}

const char * UMSPUnityMediationGetSdkVersion() {
    return strdup([kUMSVersionString UTF8String]);
}

void UMSPUnityMediationInitialize(const char *gameId, InitSuccessCallback successCallback, InitFailureCallback failCallback, const char *installId) {
    s_InitializationSuccess = successCallback;
    s_InitializationFailed = failCallback;
    NSString *convertedGameId = [NSString stringWithUTF8String:gameId];
    NSString *convertedInstallId = [NSString stringWithUTF8String:installId];
    NSString *installIdKey = @"installation_id";

    UMSPInitializeDelegate *initializationDelegate = [UMSPInitializeDelegate new];

    UMSInitializationConfiguration *initializationConfiguration = [[[[[UMSInitializationConfigurationBuilder builder]
                                                                      setGameId:convertedGameId]
                                                                     setInitializationDelegate:initializationDelegate]
                                                                    setOption:convertedInstallId
                                                                       forKey:installIdKey]
                                                                   build];

    [UMSUnityMediation initializeWithConfiguration:initializationConfiguration];
}

#ifdef __cplusplus
}
#endif
