#if CRAZY_GAMES
using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Providers.Crazy;
using CrazyGames;
using R3;

namespace AdsIntegration.Runtime.Providers
{
    internal sealed class CrazyGamesAdService<TPlacement> : IAdsProvider<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        public IAdsConfig<TPlacement> AdsConfig => CrazyGamesConfig<TPlacement>.Instance;

        private readonly Subject<Unit> _rewardedSuccess = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        public void Initialize()
        {
            CrazySDK.Ad.HasAdblock(hasAdblock => _isRewardedAvailable.OnNext(hasAdblock is false));
            CrazySDK.Ad.HasAdblock(hasAdblock => _isInterstitialAvailable.OnNext(hasAdblock is false));

            IsInitialized = true;
        }

        public void ShowRewarded(TPlacement placement)
        {
            CrazySDK.Ad.RequestAd(
                CrazyAdType.Rewarded,
                static () => Logger.Log("[CrazyGamesAdService::OnRewardedAdStarted] Showing Rewarded ads"),
                static error => Logger.LogError($"[CrazyGamesAdService::OnRewardedAdDisplayFailed] " +
                                                $"Rewarded ad display failed with {error.message} error"),
                OnRewardedAdFinished);
        }

        public void ShowInterstitial()
        {
            CrazySDK.Ad.RequestAd(
                CrazyAdType.Midgame,
                static () => Logger.Log("[CrazyGamesAdService::OnInterstitialAdStarted] " +
                                        "Showing interstitial ads"),
                static error => Logger.LogError($"[CrazyGamesAdService::OnInterstitialAdDisplayFailed] " +
                                                $"Interstitial ad display failed with {error.message} error"),
                static () =>
                    Logger.Log("[CrazyGamesAdService::OnInterstitialAdFinished] Interstitial successfully finished"));
        }

        public void PreloadRewarded()
        {
            CrazySDK.Ad.PrefetchAd(CrazyAdType.Rewarded);
        }

        public void PreloadInterstitial()
        {
            CrazySDK.Ad.PrefetchAd(CrazyAdType.Midgame);
        }

        private void OnRewardedAdFinished()
        {
            _rewardedSuccess.OnNext(Unit.Default);
            _isRewardedAvailable.OnNext(false);
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();
        }
    }
}
#endif