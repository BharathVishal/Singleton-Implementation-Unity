namespace Unity.Services.Analytics
{
    partial class AnalyticsServiceInstance
    {
        public string GetAnalyticsUserID()
        {
            return !string.IsNullOrEmpty(CustomAnalyticsId) ? CustomAnalyticsId : InstallId.GetOrCreateIdentifier();
        }
    }
}
