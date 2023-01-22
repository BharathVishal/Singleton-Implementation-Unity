using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// The exception returned when Mediation load fails.
    /// </summary>
    public class LoadFailedException : Exception
    {
        /// <summary>
        /// Accompanying error enum instance to this load exception.
        /// </summary>
        public LoadError LoadError;

        internal LoadFailedException(LoadError loadError, string message) : base(message)
        {
            LoadError = loadError;
        }
    }
}
