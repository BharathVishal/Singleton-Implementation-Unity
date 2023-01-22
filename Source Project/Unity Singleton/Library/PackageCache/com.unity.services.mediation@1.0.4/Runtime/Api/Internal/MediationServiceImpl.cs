using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Mediation.Platform;
using UnityEngine;

namespace Unity.Services.Mediation
{
    internal class MediationServiceImpl : IMediationServiceImpl
    {
        internal IPlatformMediationService platformMediationService;
        internal IDataPrivacy dataPrivacyImpl;//Interface currently used for both public and internal implementation, if they begin to deviate, two interfaces should be created
        internal IImpressionEventPublisher impressionEventPublisherImpl;//Interface currently used for both public and internal implementation, if they begin to deviate, two interfaces should be created
        TaskCompletionSource<object> m_InitializationCompletionSource;

        internal MediationServiceImpl()
        {
#if UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            platformMediationService = new EditorMediationService();
            dataPrivacyImpl = new EditorDataPrivacy();
            impressionEventPublisherImpl = new EditorImpressionEventPublisher();
#elif UNITY_ANDROID
            platformMediationService = new AndroidMediationService();
            dataPrivacyImpl = new AndroidDataPrivacy();
            impressionEventPublisherImpl = new AndroidImpressionEventPublisher();
#elif UNITY_IOS
            platformMediationService = new IosMediationService();
            dataPrivacyImpl = new IosDataPrivacy();
            impressionEventPublisherImpl = new IosImpressionEventPublisher();
#else
            platformMediationService = new UnsupportedMediationService();
            dataPrivacyImpl = new UnsupportedDataPrivacy();
            impressionEventPublisherImpl = new UnsupportedImpressionEventPublisher();
#endif
        }

        InitializationState IMediationServiceImpl.InitializationState => platformMediationService.InitializationState;

        Task IMediationServiceImpl.Initialize(string gameId, string installId)
        {
            if (!HasRequestedInitialization()
                || HasInitializationFailed())
            {
                CreateInitialization();
                platformMediationService.Initialize(gameId, installId);
            }

            return m_InitializationCompletionSource.Task;
        }

        bool HasRequestedInitialization()
        {
            return !(m_InitializationCompletionSource is null);
        }

        bool HasInitializationFailed()
        {
            return m_InitializationCompletionSource.Task.Status == TaskStatus.Faulted;
        }

        void CreateInitialization()
        {
            m_InitializationCompletionSource = new TaskCompletionSource<object>();
            platformMediationService.OnInitializationComplete += InitializationComplete;
            platformMediationService.OnInitializationFailed += InitializationFailed;
        }

        void InitializationComplete(object sender, EventArgs e)
        {
            m_InitializationCompletionSource.TrySetResult(null);
            UnsubscribeEventHandlers();
        }

        void InitializationFailed(object sender, InitializationErrorEventArgs args)
        {
            var innerException = new InitializeFailedException(args.Error, args.Message);
            var exception = new ServicesInitializationException(args.Message, innerException);
            m_InitializationCompletionSource.TrySetException(exception);
            UnsubscribeEventHandlers();
        }

        void UnsubscribeEventHandlers()
        {
            platformMediationService.OnInitializationComplete -= InitializationComplete;
            platformMediationService.OnInitializationFailed -= InitializationFailed;
        }

        public IInterstitialAd CreateInterstitialAd(string adUnitId)
        {
            return new InterstitialAd(adUnitId);
        }

        public IRewardedAd CreateRewardedAd(string adUnitId)
        {
            return new RewardedAd(adUnitId);
        }

        public IBannerAd CreateBannerAd(string adUnitId, BannerAdSize size, BannerAdAnchor anchor = BannerAdAnchor.Default, Vector2 positionOffset = new Vector2())
        {
            return new BannerAd(adUnitId, size, anchor, positionOffset);
        }

        public string SdkVersion => platformMediationService.SdkVersion;

        public IDataPrivacy DataPrivacy => dataPrivacyImpl;

        public IImpressionEventPublisher ImpressionEventPublisher => impressionEventPublisherImpl;
    }
}
