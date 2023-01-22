using System;
using UnityEngine;

namespace Unity.Services.Analytics
{
    public class AnalyticsLifetime : MonoBehaviour
    {
#if UNITY_WEBGL
        // Heartbeat more frequently on WebGL to reduce the performance impact of serialisation during batching for upload.
        const float k_HeartbeatPeriod = 20.0f;
        const float k_GameRunningPeriod = 60.0f;
#else
        const float k_HeartbeatPeriod = 60.0f;
        const float k_GameRunningPeriod = 60.0f;
#endif
        float m_HeartbeatTime = 0.0f;
        float m_GameRunningTime = 0.0f;

        internal static AnalyticsLifetime Instance { get; private set; }
        internal float TimeUntilHeartbeat => k_HeartbeatPeriod - m_HeartbeatTime;

        void Awake()
        {
            Instance = this;
            hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;

#if !UNITY_ANALYTICS_DEVELOPMENT
            hideFlags |= HideFlags.HideInInspector;
#endif

            DontDestroyOnLoad(gameObject);

            // i hate this, but i don't know of a better way to know if we're running in a test..
            var mbs = FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in mbs)
            {
                if (mb.GetType().FullName == "UnityEngine.TestTools.TestRunner.PlaymodeTestsController")
                {
                    return;
                }
            }

            AnalyticsService.internalInstance.Startup();
        }

        void Update()
        {
            // Use unscaled time in case the user sets timeScale to anything other than 1 (e.g. to 0 to pause their game),
            // we always want to record gameRunning/do batch upload on the same real-time cadence regardless of framerate
            // or user interference.

            m_GameRunningTime += Time.unscaledDeltaTime;
            if (m_GameRunningTime >= k_GameRunningPeriod)
            {
                AnalyticsService.internalInstance.RecordGameRunningIfNecessary();
                m_GameRunningTime = 0.0f;
            }

            m_HeartbeatTime += Time.unscaledDeltaTime;
            if (m_HeartbeatTime >= k_HeartbeatPeriod)
            {
                AnalyticsService.internalInstance.InternalTick();
                m_HeartbeatTime = 0.0f;
            }
        }

        void OnDestroy()
        {
            AnalyticsService.internalInstance.GameEnded();
        }
    }

    public static class ContainerObject
    {
        static bool s_Created;
        static GameObject s_Container;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Initialize()
        {
            if (!s_Created)
            {
#if UNITY_ANALYTICS_DEVELOPMENT
                Debug.Log("Created Analytics Container");
#endif

                s_Container = new GameObject("AnalyticsContainer");
                s_Container.AddComponent<AnalyticsLifetime>();

                s_Created = true;
            }
        }

        public static void DestroyContainer()
        {
            UnityEngine.Object.Destroy(s_Container);
            s_Created = false;
        }
    }
}
