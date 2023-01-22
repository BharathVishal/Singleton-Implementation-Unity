namespace Unity.Services.Mediation.Settings.Editor
{
    class GenericAdapterSettings : BaseAdapterSettings
    {
        public GenericAdapterSettings(string adapterId, bool enabled = false) : base(adapterId, enabled)
        {
            AdapterId = adapterId;
        }

        public override string AdapterId { get; }
    }
}
