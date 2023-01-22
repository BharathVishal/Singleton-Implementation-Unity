namespace Unity.Services.Mediation
{
    interface IMediationMetrics
    {
        void SendPackageInitTimeMetric(double initTimeSeconds);
    }
}
