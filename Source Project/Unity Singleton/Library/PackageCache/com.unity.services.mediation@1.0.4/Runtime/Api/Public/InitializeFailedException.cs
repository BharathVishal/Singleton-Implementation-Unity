using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// The exception returned when Mediation initialization fails.
    /// </summary>
    public class InitializeFailedException : Exception
    {
        /// <summary>
        /// Accompanying error enum instance to this initialization exception.
        /// </summary>
        public SdkInitializationError initializationError;

        internal InitializeFailedException(SdkInitializationError initializationError, string message) : base(message)
        {
            this.initializationError = initializationError;
        }
    }
}
