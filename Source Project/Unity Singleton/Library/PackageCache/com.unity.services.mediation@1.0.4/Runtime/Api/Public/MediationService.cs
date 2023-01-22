using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Mediation API for the Unity Mediation SDK.
    /// </summary>
    public static class MediationService
    {
        internal static IMediationServiceImpl s_Instance;

        /// <summary>
        /// Single entry point to all Mediation service features.
        /// </summary>
        public static IMediationService Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    throw new InvalidOperationException($"MediationService has not been initialized. Please consider calling {nameof(MediationService.Initialize)} before accessing {nameof(Instance)}");
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// The initialization state of the mediation sdk.
        /// </summary>
        public static InitializationState InitializationState => s_Instance?.InitializationState ?? InitializationState.Uninitialized;

        internal static Task Initialize(string gameId, string installId)
        {
            if (s_Instance == null)
            {
                s_Instance = new MediationServiceImpl();
            }
            return s_Instance.Initialize(gameId, installId);
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void EditorReset()
        {
            s_Instance = null;
        }
#endif
    }
}
