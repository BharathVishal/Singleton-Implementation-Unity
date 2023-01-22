using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Mediation
{
    class MediationMetrics : IMediationMetrics
    {

        internal const string k_PackageName = "com.unity.services.mediation";
        const string k_PackageInitTimeKey = "package_init_time";

        readonly IMetrics m_Metrics;

        internal MediationMetrics(IMetricsFactory metricsFactory)
        {
            m_Metrics = metricsFactory.Create(k_PackageName);
        }

        public void SendPackageInitTimeMetric(double initTimeSeconds)
        {
            m_Metrics.SendHistogramMetric(k_PackageInitTimeKey, initTimeSeconds);
        }
    }
}
