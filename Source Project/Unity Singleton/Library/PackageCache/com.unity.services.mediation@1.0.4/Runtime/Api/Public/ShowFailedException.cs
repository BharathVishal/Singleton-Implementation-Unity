using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// The exception returned when Mediation show fails.
    /// </summary>
    public class ShowFailedException : Exception
    {
        /// <summary>
        /// Accompanying error enum instance to this show exception.
        /// </summary>
        public ShowError ShowError;

        internal ShowFailedException(ShowError showError, string message) : base(message)
        {
            ShowError = showError;
        }
    }
}
