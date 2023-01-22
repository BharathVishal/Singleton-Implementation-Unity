#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Unity.Services.Mediation.Platform
{
    class EditorMediationService : IPlatformMediationService
    {
#pragma warning disable 67
        public event EventHandler OnInitializationComplete;

        public event EventHandler<InitializationErrorEventArgs> OnInitializationFailed;
#pragma warning restore 67

        public InitializationState InitializationState { get; private set; } = InitializationState.Uninitialized;
        public string SdkVersion => "0.0.0";

        public EditorMediationService()
        {
        }

        public void Initialize(string gameId, string installId)
        {
            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            if (activeBuildTarget != BuildTarget.Android && activeBuildTarget != BuildTarget.iOS)
            {
                Debug.LogWarning($"The selected build platform is not supported by Mediation. Build Target: {activeBuildTarget.ToString()}. Using Temporary GameId.");
                gameId = "EDITOR";
            }

            if (!string.IsNullOrEmpty(gameId))
            {
                InitializationState = InitializationState.Initialized;
                OnInitializationComplete?.Invoke(null, EventArgs.Empty);
            }
            else
            {
                InitializationState = InitializationState.Uninitialized;
                OnInitializationFailed?.Invoke(null, new InitializationErrorEventArgs(SdkInitializationError.Unknown, "Game Id was Empty."));
            }
        }
    }
}
#endif
