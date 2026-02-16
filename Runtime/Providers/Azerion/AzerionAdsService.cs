#if AZERION
using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdsIntegration.Runtime.Providers.Azerion
{
    internal sealed class AzerionAdsService<TPlacement> : IAdsProvider<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        public IAdsConfig<TPlacement> AdsConfig => AzerionConfig<TPlacement>.Instance;

        private readonly Subject<Unit> _rewardedSuccess = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        public void Initialize()
        {
            var gameDistribution = ResourceLoader<GameDistribution>.Load(nameof(GameDistribution));
            var createdPrefab = Object.Instantiate(gameDistribution);
            createdPrefab.name = nameof(GameDistribution);

            GameDistribution.Instance.GAME_KEY = AzerionConfig<TPlacement>.Instance.GameKey;

            GameDistribution.Instance.PreloadRewardedAd();

            GameDistribution.OnResumeGame += ResumeGame;
            GameDistribution.OnPauseGame += PauseGame;
            GameDistribution.OnRewardedVideoSuccess += OnRewardedAdFinished;
            GameDistribution.OnRewardedVideoFailure += OnRewardedAdDisplayFailed;

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

        public void PreloadInterstitial() { }

        private void OnRewardedAdDisplayFailed()
        {
            _isRewardedAvailable.OnNext(false);
        }

        private void OnRewardedAdFinished()
        {
            _rewardedSuccess?.OnNext(Unit.Default);
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
        }
    }
}
#endif