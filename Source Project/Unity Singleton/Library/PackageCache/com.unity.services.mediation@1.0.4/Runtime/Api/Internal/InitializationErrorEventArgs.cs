using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// InitializationError Event Arguments
    /// </summary>
    class InitializationErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initialization error reported by underlying SDK
        /// </summary>
        internal SdkInitializationError Error { get; }

        /// <summary>
        /// Error message reported by underlying SDK
        /// </summary>
        internal string Message { get; }

        /// <summary>
        /// Constructs Arguments for Initialization Errors
        /// </summary>
        /// <param name="error">Underlying SDK Initialization Error Enum</param>
        /// <param name="message">Underlying SDK Initialization Error Message</param>
        internal InitializationErrorEventArgs(SdkInitializationError error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
