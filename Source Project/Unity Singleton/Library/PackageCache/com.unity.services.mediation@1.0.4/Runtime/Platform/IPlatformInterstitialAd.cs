using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Interface of an Interstitial Ad.
    /// </summary>
    interface IPlatformInterstitialAd : IDisposable
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
        /// Event to be triggered by the adapter when an Ad is started.
        /// </summary>
        event EventHandler OnShowed;

        /// <summary>
        /// Event to be triggered by the adapter when the user clicks on the Ad.
        /// </summary>
        event EventHandler OnClicked;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad is closed.
        /// </summary>
        event EventHandler OnClosed;

        /// <summary>
        /// Event to be triggered by the adapter when the Ad has an error.
        /// </summary>
        event EventHandler<ShowErrorEventArgs> OnFailedShow;

        /// <summary>
        /// Get the current state of the ad.
        /// </summary>
        AdState AdState { get; }

        /// <summary>
        /// Get the ad unit id set during construction.
        /// </summary>
        string AdUnitId { get; }

        /// <summary>
        /// Method to tell the Mediation SDK to load an Ad.
        /// </summary>
        void Load();

        /// <summary>
        /// Method to tell the Mediation SDK to show the loaded Ad.
        /// </summary>
        void Show();
    }
}
