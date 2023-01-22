#if !UNITY_ANDROID && !UNITY_IOS
using System;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedMediationService : IPlatformMediationService
    {
        public event EventHandler OnInitializationComplete;
#pragma warning disable 67
        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;
#pragma warning restore 67
        public InitializationState InitializationState { get; private set; } = InitializationState.Uninitialized;

        public string SdkVersion => "0.0.0";

        public void Initialize(string gameId, string installId)
        {
            InitializationState = InitializationState.Initialized;
            OnInitializationComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}
#endif
