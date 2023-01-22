namespace Unity.Services.Mediation.Platform
{
    internal interface IExternallyInvokableEventPublisher
    {
        void InvokeOnImpressionEvent(object sender, ImpressionEventArgs args);
    }
}
