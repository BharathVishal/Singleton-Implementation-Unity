using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// ShowError Event Arguments.
    /// </summary>
    public class ShowErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Enum for this show error event.
        /// </summary>
        public ShowError Error { get; }

        /// <summary>
        /// String message for this show error event.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Constructor for Show Error Events Arguments.
        /// </summary>
        /// <param name="error">Error enum for this event argument.</param>
        /// <param name="message">Error message for this event argument.</param>
        public ShowErrorEventArgs(ShowError error, string message)
        {
            Error = error;
            Message = message;
        }
    }
}
