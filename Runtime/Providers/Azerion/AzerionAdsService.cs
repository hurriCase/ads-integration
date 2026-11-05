#if AZERION
using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdsIntegration.Runtime.Providers.Azerion
{
    [PublicAPI]
    public sealed class AzerionAdsService<TPlacement> : IAdsProvider<TPlacement>
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

        private readonly AzerionConfig<TPlacement> _adsConfig;

        private bool _isInterstitialRequested;
        private bool _isInterstitialDisplayed;

        public AzerionAdsService(AzerionConfig<TPlacement> adsConfig)
        {
            _adsConfig = adsConfig;
        }

        public void Initialize()
        {
            var gameDistribution = ResourceLoader<GameDistribution>.Load(nameof(GameDistribution));
            var createdPrefab = Object.Instantiate(gameDistribution);
            createdPrefab.name = nameof(GameDistribution);

            GameDistribution.Instance.GAME_KEY = _adsConfig.GameKey;

            GameDistribution.Instance.PreloadRewardedAd();

            GameDistribution.OnResumeGame += ResumeGame;
            GameDistribution.OnPauseGame += PauseGame;
            GameDistribution.OnRewardedVideoSuccess += OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure += OnRewardedAdDisplayFailed;
            GameDistribution.OnPreloadRewardedVideo += OnRewardedVideoPreloaded;

            IsInitialized = true;
        }

        public void ShowRewarded(TPlacement placement)
        {
            GameDistribution.Instance.ShowRewardedAd();
        }

        public void ShowInterstitial()
        {
            _isInterstitialRequested = true;
            GameDistribution.Instance.ShowAd();
        }

        public void PreloadRewarded()
        {
            GameDistribution.Instance.PreloadRewardedAd();
        }

        private void OnRewardedVideoPreloaded(int loaded)
        {
            _isRewardedAvailable.OnNext(loaded == 1);
        }

        public void PreloadInterstitial() { }

        private void OnRewardedAdDisplayFailed()
        {
            _isRewardedAvailable.OnNext(false);
        }

        private void OnRewardedAdFinished()
        {
            _rewardedSuccess.OnNext(Unit.Default);
            _isRewardedAvailable.OnNext(false);
        }

        // GameDistribution reports every ad type through the same pause and resume events,
        // and has no interstitial callbacks, so the request flag tells interstitials from rewarded ads.
        private void PauseGame()
        {
            Time.timeScale = 0;

            if (!_isInterstitialRequested)
                return;

            _isInterstitialRequested = false;
            _isInterstitialDisplayed = true;
            _interstitialDisplayed.OnNext(Unit.Default);
        }

        private void ResumeGame()
        {
            Time.timeScale = 1;

            _isInterstitialRequested = false;

            if (!_isInterstitialDisplayed)
                return;

            _isInterstitialDisplayed = false;
            _interstitialClosed.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
            _interstitialDisplayed.Dispose();
            _interstitialClosed.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();

            GameDistribution.OnResumeGame -= ResumeGame;
            GameDistribution.OnPauseGame -= PauseGame;
            GameDistribution.OnRewardedVideoSuccess -= OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure -= OnRewardedAdDisplayFailed;
            GameDistribution.OnPreloadRewardedVideo -= OnRewardedVideoPreloaded;
        }
    }
}
#endif