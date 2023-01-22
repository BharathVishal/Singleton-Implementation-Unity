using System;
using UnityEngine.Purchasing.Interfaces;
using UnityEngine.Purchasing.Telemetry;

namespace UnityEngine.Purchasing
{
    class MetricizedGooglePlayStoreExtensions : GooglePlayStoreExtensions
    {
        readonly ITelemetryMetricsService m_TelemetryMetricsService;


        internal MetricizedGooglePlayStoreExtensions(IGooglePlayStoreService googlePlayStoreService,
            IGooglePlayStoreFinishTransactionService googlePlayStoreFinishTransactionService, ILogger logger,
            ITelemetryDiagnostics telemetryDiagnostics, ITelemetryMetricsService telemetryMetricsService)
            : base(googlePlayStoreService, googlePlayStoreFinishTransactionService, logger, telemetryDiagnostics)
        {
            m_TelemetryMetricsService = telemetryMetricsService;
        }

        public override void UpgradeDowngradeSubscription(string oldSku, string newSku,
            GooglePlayProrationMode desiredProrationMode)
        {
            m_TelemetryMetricsService.ExecuteTimedAction(
                () => base.UpgradeDowngradeSubscription(oldSku, newSku, desiredProrationMode),
                TelemetryMetricDefinitions.upgradeDowngradeSubscriptionName);
        }

        public override void RestoreTransactions(Action<bool> callback)
        {
            m_TelemetryMetricsService.ExecuteTimedAction(
                () => base.RestoreTransactions(callback), TelemetryMetricDefinitions.restoreTransactionName);
        }
    }
}
