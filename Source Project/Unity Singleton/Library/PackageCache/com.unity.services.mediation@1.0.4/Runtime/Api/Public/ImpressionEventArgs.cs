using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Event args fired with impression events from the ImpressionEventPublisher.
    /// </summary>
    public class ImpressionEventArgs : EventArgs
    {
        /// <summary>
        /// AdUnitId for Impression Events.
        /// </summary>
        public string AdUnitId { get; }

        /// <summary>
        /// Impression Data for Impression Events.
        /// </summary>
        public ImpressionData ImpressionData { get; }

        /// <summary>
        /// Constructs Arguments for Impression Events
        /// </summary>
        /// <param name="adUnitId">Ad Unit Id for ad that fired impression event.</param>
        /// <param name="impressionData">Additional data about this impression event.</param>
        public ImpressionEventArgs(string adUnitId, ImpressionData impressionData)
        {
            AdUnitId = adUnitId;
            ImpressionData = impressionData;
        }
    }
}
