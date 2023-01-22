namespace Unity.Services.Mediation
{
    /// <summary>
    /// Enum for show error codes.
    /// </summary>
    public enum ShowError
    {
        /// <summary>
        /// Unknown error, see message.
        /// </summary>
        Unknown,

        /// <summary>
        /// Ad wasn't loaded and cannot be shown.
        /// </summary>
        AdNotLoaded,

        /// <summary>
        /// Ad network adapter error caused ad to not show.
        /// </summary>
        AdNetworkError,

        /// <summary>
        /// Android Only: The activity passed to the ad unit was invalid.
        /// </summary>
        InvalidActivity
    }
}
