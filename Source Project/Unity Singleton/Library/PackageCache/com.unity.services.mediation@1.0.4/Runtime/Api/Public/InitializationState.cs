namespace Unity.Services.Mediation
{
    /// <summary>
    /// <para>InitializationState enum denoting the initialization status of Unity Mediation.</para>
    ///
    /// <para>There are two initialization flows that can happen.
    /// Successful Initialization : Uninitialized -&gt; Initializing -&gt; Initialized</para>
    ///
    /// <para>Failed Initialization : Uninitialized -&gt; Initializing -&gt; Uninitialized</para>
    /// </summary>
    public enum InitializationState
    {
        /// <summary>
        /// SDK is Uninitialized and can be initialized in this state.
        /// </summary>
        Uninitialized,

        /// <summary>
        /// SDK is Initializing and cannot be initialized in this state.
        /// </summary>
        Initializing,

        /// <summary>
        /// SDK is Initialized and should not be initialized in this state.
        /// </summary>
        Initialized
    }
}
