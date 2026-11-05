#if CRAZY_GAMES
using System;
using AdsIntegration.Runtime.Base;
using CrazyGames;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime.Providers.Crazy
{
    [PublicAPI]
    public sealed class CrazyGamesAdsProvider<TPlacement> : IAdsProvider<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;
        public Observable<Unit> OnInterstitialDisplayed => _interstitialDisplayed;
        public Observable<Unit> OnInterstitialClosed => _interstitialClosed;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private readonly Subject<Unit> _rewardedSuccess = new();
        private readonly Subject<Unit> _interstitialDisplayed = new();
        private readonly Subject<Unit> _interstitialClosed = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        public void Initialize()
        {
            CrazySDK.Ad.HasAdblock(hasAdblock =>
            {
                _isRewardedAvailable.OnNext(!hasAdblock);
                _isInterstitialAvailable.OnNext(!hasAdblock);
            });

            IsInitialized = true;
        }

        public void ShowRewarded(TPlacement placement)
        {
            CrazySDK.Ad.RequestAd(
                CrazyAdType.Rewarded,
                static () => Logger.Log("[CrazyGamesAdsProvider::OnRewardedAdStarted] Showing Rewarded ads"),
                static error => Logger.LogError("[CrazyGamesAdsProvider::OnRewardedAdDisplayFailed] " +
                                                $"Rewarded ad display failed with {error.message} error"),
                OnRewardedAdFinished);
        }

        public void ShowInterstitial()
        {
            CrazySDK.Ad.RequestAd(
                CrazyAdType.Midgame,
                OnInterstitialAdStarted,
                static error => Logger.LogError("[CrazyGamesAdsProvider::OnInterstitialAdDisplayFailed] " +
                                                $"Interstitial ad display failed with {error.message} error"),
                OnInterstitialAdFinished);
        }

        public void PreloadRewarded()
        {
            CrazySDK.Ad.PrefetchAd(CrazyAdType.Rewarded);
        }

        public void PreloadInterstitial()
        {
            CrazySDK.Ad.PrefetchAd(CrazyAdType.Midgame);
        }

        private void OnInterstitialAdStarted()
        {
            Logger.Log("[CrazyGamesAdsProvider::OnInterstitialAdStarted] Showing interstitial ads");
            _interstitialDisplayed.OnNext(Unit.Default);
        }

        private void OnInterstitialAdFinished()
        {
            Logger.Log("[CrazyGamesAdsProvider::OnInterstitialAdFinished] Interstitial successfully finished");
            _interstitialClosed.OnNext(Unit.Default);
        }

        private void OnRewardedAdFinished()
        {
            _rewardedSuccess.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
            _interstitialDisplayed.Dispose();
            _interstitialClosed.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();
        }
    }
}
#endif