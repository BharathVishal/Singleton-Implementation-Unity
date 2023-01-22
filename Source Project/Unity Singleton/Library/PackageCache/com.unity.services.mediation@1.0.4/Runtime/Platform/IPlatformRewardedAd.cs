using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Interface of a Rewarded Ad on the platform side.
    /// </summary>
    interface IPlatformRewardedAd : IDisposable
    {
        /// <summary>
        /// Event to be triggered by the adapter when an Ad is loaded.
        /// </summary>
        event EventHandler OnLoaded;

        /// <summary>
        /// Event to be triggered by the adapter when an Ad fails to load.
        /// </summary>
        event EventHandler<LoadErrorEventArgs> OnFailedLoad;

        /// <summary>
        /// Event to be triggered by the adapter when a Rewarded Ad is shown.
        /// </summary>
        event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the RewardedAd.
        /// </summary>
        event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd is closed.
        /// </summary>
        event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the RewardedAd has an error.
        /// </summary>
        event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Event to be triggered by the adapter when a reward needs to be issued.
        /// </summary>
        event EventHandler<RewardEventArgs> OnUserRewarded;

        /// <summary>
        ///<value>Gets the state of the <c>RewardedAd</c></value>
        /// </summary>
        AdState AdState { get; }

        /// <summary>
        /// <value>Gets the id of the ad unit.</value>
        /// </summary>
        string AdUnitId { get; }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        void Load();

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        /// <param name="showOptions">Optional, allows setting optional parameters for showing a rewarded ad.</param>
        void Show(RewardedAdShowOptions showOptions = null);
    }
}
