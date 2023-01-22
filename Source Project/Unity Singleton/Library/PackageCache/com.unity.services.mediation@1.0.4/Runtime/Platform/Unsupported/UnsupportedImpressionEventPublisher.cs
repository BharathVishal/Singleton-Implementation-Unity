#if !UNITY_ANDROID && !UNITY_IOS
using System;

namespace Unity.Services.Mediation.Platform
{
    class UnsupportedImpressionEventPublisher : IImpressionEventPublisher
    {
#pragma warning disable 67
        public event EventHandler<ImpressionEventArgs> OnImpression;
#pragma warning restore 67
    }
}
#endif
