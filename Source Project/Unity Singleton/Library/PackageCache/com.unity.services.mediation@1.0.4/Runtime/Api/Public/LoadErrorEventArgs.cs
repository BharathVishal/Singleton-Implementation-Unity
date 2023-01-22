using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// LoadError Event Arguments.
    /// </summary>
    public class LoadErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Underlying SDK Load Error.
        /// </summary>
        public LoadError Error { get; }

        /// <summary>
        /// Underlying SDK Load Error Message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructor for Load Error Event Arguments.
        /// </summary>
        /// <param name="error">Underlying SDK Load Error Enum.</param>
        /// <param name="message">Underlying SDK Load Error String.</param>
        public LoadErrorEventArgs(LoadError error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
