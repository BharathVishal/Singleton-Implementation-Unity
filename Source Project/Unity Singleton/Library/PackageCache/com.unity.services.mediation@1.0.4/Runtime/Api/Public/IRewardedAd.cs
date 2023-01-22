using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Interface of a Rewarded Ad.
    /// </summary>
    public interface IRewardedAd : IDisposable
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
        /// <returns>LoadAsync Task</returns>
        /// <exception cref="Unity.Services.Mediation.LoadFailedException">Thrown when the ad failed to load</exception>
        Task LoadAsync();

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        /// <param name="showOptions">Optional, allows setting optional parameters for showing a rewarded ad.</param>
        /// <returns>ShowAsync Task</returns>
        /// <exception cref="Unity.Services.Mediation.ShowFailedException">Thrown when the ad failed to show</exception>
        Task ShowAsync(RewardedAdShowOptions showOptions = null);
    }

    /// <summary>
    /// Contains optional parameters for showing a rewarded ad.
    /// </summary>
    public class RewardedAdShowOptions
    {
        /// <summary>
        /// Allows providing data for a server to server redeem callback.
        /// </summary>
        public S2SRedeemData S2SData { get; set; }

        /// <summary>
        /// If set to true, the ad will automatically load another ad after it is done showing, so it is not necessary
        /// to call LoadAsync again. This will occur when the OnClosed or OnFailedShow event is triggered.
        /// </summary>
        public bool AutoReload { get; set; } = false;
    }

    /// <summary>
    /// Contains the Server to Server redeem callback data.
    /// Make sure the rewarded ad is properly configured for server to server redeem callback authentication and a server URL is set in the dashboard.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct S2SRedeemData
    {
        /// <summary>
        /// Unique identifier of the user being rewarded
        /// </summary>
        public string UserId;

        /// <summary>
        /// Optional custom data provided to the server
        /// For example in a json format: "{\"reward\":\"Gems\",\"amount\":20}"
        /// </summary>
        public string CustomData;
    }
}
