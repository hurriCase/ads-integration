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

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private readonly Subject<Unit> _rewardedSuccess = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        private readonly AzerionConfig<TPlacement> _adsConfig;

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

        private void ResumeGame()
        {
            Time.timeScale = 1;
        }

        private void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
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