#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Services.Mediation.Platform
{
    class IosMediationService : IPlatformMediationService
    {
        static IosMediationService instance;

        delegate void InitializeSuccessCallback();
        delegate void InitializeFailCallback(int error, string message);

        static readonly InitializeSuccessCallback k_InitializeSuccessCallback = InitializationSuccess;
        static readonly InitializeFailCallback    k_InitializeFailCallback    = InitializationFailed;

        public event EventHandler OnInitializationComplete;
        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;

        public InitializationState InitializationState => UnityMediationGetInitializationState();

        public string SdkVersion => UnityMediationGetSdkVersion();

        public void Initialize(string gameId, string installId)
        {
            instance = this;
            UnityMediationInitialize(gameId, k_InitializeSuccessCallback, k_InitializeFailCallback, installId);
        }

        [DllImport("__Internal", EntryPoint = "UMSPUnityMediationGetSdkVersion")]
        static extern string UnityMediationGetSdkVersion();

        [DllImport("__Internal", EntryPoint = "UMSPUnityMediationGetInitializationState")]
        static extern InitializationState UnityMediationGetInitializationState();

        [DllImport("__Internal", EntryPoint = "UMSPUnityMediationInitialize")]
        static extern void UnityMediationInitialize(string gameId, InitializeSuccessCallback successCallback, InitializeFailCallback failCallback, string installId);

        [MonoPInvokeCallback(typeof(InitializeSuccessCallback))]
        static void InitializationSuccess()
        {
            ThreadUtil.Post(state => instance.OnInitializationComplete?.Invoke(null, EventArgs.Empty));
        }

        [MonoPInvokeCallback(typeof(InitializeFailCallback))]
        static void InitializationFailed(int error, string message)
        {
            var eventArgs = new InitializationErrorEventArgs((SdkInitializationError)error, message);
            ThreadUtil.Post(state => instance.OnInitializationFailed?.Invoke(null, eventArgs));
        }
    }
}
#endif
