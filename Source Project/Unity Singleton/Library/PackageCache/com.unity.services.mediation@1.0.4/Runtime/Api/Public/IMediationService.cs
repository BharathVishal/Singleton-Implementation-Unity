using UnityEngine;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// The interface for Mediation features.
    /// </summary>
    public interface IMediationService
    {
        /// <summary>
        /// The Interstitial ads creator function.
        /// </summary>
        /// <param name="adUnitId"> The Ad Unit Id for the ad unit you wish to show. </param>
        /// <returns> A new Interstitial Ad instance. </returns>
        IInterstitialAd CreateInterstitialAd(string adUnitId);

        /// <summary>
        /// The Rewarded ads creator function.
        /// </summary>
        /// <param name="adUnitId"> The Ad Unit Id for the ad unit you wish to show. </param>
        /// <returns> A new Rewarded Ad instance. </returns>
        IRewardedAd CreateRewardedAd(string adUnitId);

        /// <summary>
        /// The Banner ads creator function.
        /// </summary>
        /// <param name="adUnitId">Unique Id for the Ad you want to show.</param>
        /// <param name="size">Size of banner set to be constructed.</param>
        /// <param name="anchor">Anchor on which the banner position is based</param>
        /// <param name="positionOffset">The X, Y coordinates offsets, relative to the anchor point</param>
        /// <returns> A new Banner Ad instance. </returns>
        IBannerAd CreateBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2());

        /// <summary>
        /// Access the Data Privacy API, to register the user's consent status.
        /// </summary>
        IDataPrivacy DataPrivacy { get; }

        /// <summary>
        /// Access the Impression Event Publisher API, to receive events when impression events are fired from ad objects.
        /// </summary>
        IImpressionEventPublisher ImpressionEventPublisher { get; }

        /// <summary>
        /// Native Mediation SDK version this mediation service is operating upon.
        /// </summary>
        string SdkVersion { get; }
    }
}
