using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// An interface that can pass on events when impression events are fired from ad objects.
    /// </summary>
    public interface IImpressionEventPublisher
    {
        /// <summary>
        /// Event to subscribe to when listening for impression events.
        /// </summary>
        event EventHandler<ImpressionEventArgs> OnImpression;
    }
}
