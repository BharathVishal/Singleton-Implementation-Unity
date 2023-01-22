using System;

namespace Unity.Services.Mediation.Platform
{
    interface IPlatformMediationService
    {
        event EventHandler OnInitializationComplete;
        event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;

        InitializationState InitializationState { get; }

        string SdkVersion { get; }

        void Initialize(string gameId, string installId);
    }
}
