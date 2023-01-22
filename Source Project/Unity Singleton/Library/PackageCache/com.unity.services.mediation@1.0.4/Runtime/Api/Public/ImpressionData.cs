using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Impression Data associated with an Impression Event.
    /// </summary>
    [Serializable]
    public class ImpressionData
    {
        /// <summary>
        /// The timestamp for the impression event.
        /// </summary>
        public string Timestamp;

        /// <summary>
        /// Ad unit name as defined by the publisher.
        /// </summary>
        public string AdUnitName;

        /// <summary>
        /// The adUnitId for the impression event.
        /// </summary>
        public string AdUnitId;

        /// <summary>
        /// The adUnitFormat for the impression event.
        /// </summary>
        public string AdUnitFormat;

        /// <summary>
        /// The impressionId for the impression event.
        /// </summary>
        public string ImpressionId;

        /// <summary>
        /// The currency for the impression event.
        /// </summary>
        public string Currency;

        /// <summary>
        /// The revenue accuracy for the impression event.
        /// </summary>
        public string RevenueAccuracy;

        /// <summary>
        /// Returns the single impression publisher revenue for an impression event. For example, a CPM of 2.55 will return a value of 0.00255 for that impression.
        /// </summary>
        public double PublisherRevenuePerImpression;

        /// <summary>
        /// Returns the single impression publisher revenue for an impression event. For example, a single impression value of 0.00255 will return a value of 2550 for that impression.
        /// </summary>
        public Int64 PublisherRevenuePerImpressionInMicros;

        /// <summary>
        /// Name of Ad Network serving the impression, for example the ad_network_id.
        /// </summary>
        public string AdSourceName;

        /// <summary>
        /// ID of the the Ad Networks ad unit/placement/zone.
        /// </summary>
        public string AdSourceInstance;

        /// <summary>
        /// The sdk app version for the impression event.
        /// </summary>
        public string AppVersion;

        /// <summary>
        /// The line item id for the impression event.
        /// </summary>
        public string LineItemId;

        /// <summary>
        /// The line item name for the impression event.
        /// </summary>
        public string LineItemName;

        /// <summary>
        /// The line item priority for the impression event.
        /// </summary>
        public string LineItemPriority;

        /// <summary>
        /// The country code for the impression event.
        /// </summary>
        public string Country;
    }
}
