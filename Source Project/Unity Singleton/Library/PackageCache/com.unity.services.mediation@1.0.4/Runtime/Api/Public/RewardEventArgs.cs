using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Reward Event Arguments.
    /// </summary>
    public class RewardEventArgs : EventArgs
    {
        /// <summary>
        /// Reward Type for Rewarded Ad Event.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Reward Amount for Rewarded Ad Event.
        /// </summary>
        public string Amount { get; }

        /// <summary>
        /// Constructor for Arguments for Rewarded Event.
        /// </summary>
        /// <param name="type">Type of Reward.</param>
        /// <param name="amount">Amount of Reward.</param>
        public RewardEventArgs(string type, string amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
