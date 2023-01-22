using System;

namespace Unity.Services.Mediation
{
    /// <summary>
    /// Predefined size for a Banner Ad.
    /// </summary>
    public enum BannerAdPredefinedSize
    {
        /// <summary>
        /// Standard banner size, 320x50 dp
        /// </summary>
        Banner,

        /// <summary>
        /// Large banner size, 320x100 dp
        /// </summary>
        LargeBanner,

        /// <summary>
        /// Medium Rectangle banner size, 300x250 dp
        /// </summary>
        MediumRectangle,

        /// <summary>
        /// Leaderboard banner size, 728x90 dp
        /// </summary>
        Leaderboard
    }

    /// <summary>
    /// Extension for the BannerAdPredefinedSize enumeration to provide banner Ad sizes.
    /// </summary>
    public static class BannerAdPredefinedSizeExtension
    {
        /// <summary>
        /// Uses a BannerAdPredefinedSize to create a BannerAdSize
        /// </summary>
        /// <param name="bannerAdPredefinedSize"></param>
        /// <returns>BannerAdSize</returns>
        public static BannerAdSize ToBannerAdSize(this BannerAdPredefinedSize bannerAdPredefinedSize)
        {
            switch (bannerAdPredefinedSize)
            {
                case BannerAdPredefinedSize.Banner:
                    return BannerAdSize.FromDpUnits(320, 50);
                case BannerAdPredefinedSize.LargeBanner:
                    return BannerAdSize.FromDpUnits(320, 100);
                case BannerAdPredefinedSize.MediumRectangle:
                    return BannerAdSize.FromDpUnits(300, 250);
                case BannerAdPredefinedSize.Leaderboard:
                    return BannerAdSize.FromDpUnits(728, 90);
                default:
                    throw new ArgumentOutOfRangeException(nameof(bannerAdPredefinedSize), bannerAdPredefinedSize, null);
            }
        }
    }
}
