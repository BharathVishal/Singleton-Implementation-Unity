#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation.Platform
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct IosImpressionData
    {
        public string Timestamp;
        public string AdUnitName;
        public string AdUnitId;
        public string AdUnitFormat;
        public string ImpressionId;
        public string Currency;
        public string RevenueAccuracy;
        public double PublisherRevenuePerImpression;
        public Int64  PublisherRevenuePerImpressionInMicros;
        public string AdSourceName;
        public string AdSourceInstance;
        public string AppVersion;
        public string LineItemId;
        public string LineItemName;
        public string LineItemPriority;
        public string Country;

        public ImpressionData ToImpressionData()
        {
            return new ImpressionData
            {
                Timestamp = Timestamp,
                AdUnitName = AdUnitName,
                AdUnitId = AdUnitId,
                AdUnitFormat = AdUnitFormat,
                ImpressionId = ImpressionId,
                Currency = Currency,
                RevenueAccuracy = RevenueAccuracy,
                PublisherRevenuePerImpression = PublisherRevenuePerImpression,
                PublisherRevenuePerImpressionInMicros = PublisherRevenuePerImpressionInMicros,
                AdSourceName = AdSourceName,
                AdSourceInstance = AdSourceInstance,
                AppVersion = AppVersion,
                LineItemId = LineItemId,
                LineItemName = LineItemName,
                LineItemPriority = LineItemPriority,
                Country = Country
            };
        }
    }
}
#endif
