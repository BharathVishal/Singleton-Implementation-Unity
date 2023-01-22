namespace Unity.Services.Mediation
{
    /// <summary>
    /// The current state of an Interstitial or rewarded Ad.
    /// The current state of an Ad load or show.<para/>
    ///
    /// <para>Unloaded: Indicates that an ad unit is ready to be loaded. AdUnits that are Unloaded cannot
    /// be showed.</para>
    ///
    /// <para>Set when Ad is instanced, Ad has closed or failed on show, Ad has failed to Load.</para>
    ///
    /// <para>Loading: Indicates that an ad unit is in the process of loading ad data. AdUnits that
    /// are Loading cannot be loaded again until a subsequent load has failed or
    /// show has completed/failed.</para>
    ///
    /// <para>Set when Ad load is called.</para>
    ///
    /// <para>Loaded: Indicates that an ad unit has loaded ad data and is ready to show. AdUnits that
    /// are Loaded cannot be loaded again until a subsequent show has completed/failed.</para>
    ///
    /// <para>Set when a line item has loaded (Ad load has succeeded).</para>
    ///
    /// <para>Showing:  Indicates that an ad unit is in the process of showing. AdUnits that are Showing
    /// cannot be loaded or showed again until playback has completed/failed.</para>
    ///
    /// <para>Set when Ad show is called.</para>
    ///
    /// <para>Potential State Transitions:
    /// Unloaded to Loading to Loaded to Showing to Unloaded
    /// Unloaded to Loading to Unloaded</para>
    /// </summary>
    public enum AdState
    {
        /// <summary>
        /// Indicates that an ad unit is ready to be loaded. AdUnits that are Unloaded cannot
        /// be showed.
        /// </summary>
        Unloaded,

        /// <summary>
        /// Indicates that an ad unit is in the process of loading ad data. AdUnits that
        /// are Loading cannot be loaded again until a subsequent load has failed or
        /// show has completed/failed.
        /// </summary>
        Loading,

        /// <summary>
        /// Indicates that an ad unit has loaded ad data and is ready to show. AdUnits that
        /// are Loaded cannot be loaded again until a subsequent show has completed/failed.
        /// </summary>
        Loaded,

        /// <summary>
        /// Indicates that an ad unit is in the process of showing. AdUnits that are Showing
        /// cannot be loaded or showed again until playback has completed/failed.
        /// </summary>
        Showing
    }
}
