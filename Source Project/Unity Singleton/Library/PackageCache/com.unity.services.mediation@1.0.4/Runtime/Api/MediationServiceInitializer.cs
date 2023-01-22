using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Debug = UnityEngine.Debug;

namespace Unity.Services.Mediation
{
    class MediationServiceInitializer : IInitializablePackage
    {
        internal const string keyGameId = "com.unity.ads.game-id";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            CoreRegistry.Instance.RegisterPackage(new MediationServiceInitializer())
                .DependsOn<IInstallationId>()
                .DependsOn<IProjectConfiguration>()
                .DependsOn<IMetricsFactory>();
        }

        public async Task Initialize(CoreRegistry registry)
        {
            var stopwatch = new Stopwatch();

            var metricsFactory = registry.GetServiceComponent<IMetricsFactory>();
            var metrics        = new MediationMetrics(metricsFactory);

            try
            {
                stopwatch.Start();
                var installationId = registry.GetServiceComponent<IInstallationId>();
                var projectConfig  = registry.GetServiceComponent<IProjectConfiguration>();

                await Initialize(installationId, projectConfig);
            }
            finally
            {
                stopwatch.Stop();
            }

            metrics.SendPackageInitTimeMetric(stopwatch.Elapsed.TotalSeconds);
        }

        internal async Task Initialize(IInstallationId installationId, IProjectConfiguration projectConfiguration)
        {
            string installId = installationId.GetOrCreateIdentifier();
            string gameId    = projectConfiguration.GetString(keyGameId);

#if UNITY_ANDROID || UNITY_IOS
            if (!Application.isEditor && string.IsNullOrEmpty(gameId))
            {
                Debug.LogError("No gameId was set for the mediation service. Please make sure your project is linked to the dashboard when you build your application.");
            }
#endif
            await MediationService.Initialize(gameId, installId);
        }
    }
}
