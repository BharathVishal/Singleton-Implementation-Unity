namespace Unity.Services.Mediation
{
    /// <summary>
    /// Banner anchors
    /// </summary>
    public enum BannerAdAnchor
    {
        /// <summary>
        /// Top Center of Screen.
        /// </summary>
        TopCenter = 0,

        /// <summary>
        /// Top Left of Screen.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        /// Top Right of Screen.
        /// </summary>
        TopRight = 2,

        /// <summary>
        /// Center of Screen.
        /// </summary>
        Center = 3,

        /// <summary>
        ///Middle Left of Screen.
        /// </summary>
        MiddleLeft = 4,

        /// <summary>
        /// Middle Right of Screen.
        /// </summary>
        MiddleRight = 5,

        /// <summary>
        /// Bottom Center of Screen.
        /// </summary>
        BottomCenter = 6,

        /// <summary>
        /// Bottom Left of Screen.
        /// </summary>
        BottomLeft = 7,

        /// <summary>
        /// Bottom Right of Screen.
        /// </summary>
        BottomRight = 8,

        /// <summary>
        /// Use offset as screen coordinates.
        /// </summary>
        None = 9,

        /// <summary>
        /// Indicates the default anchor if none is provided.
        /// </summary>
        Default = TopCenter
    }
}
