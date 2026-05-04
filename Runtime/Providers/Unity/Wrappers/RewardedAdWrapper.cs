#if LEVEL_PLAY
using System;
using AdsIntegration.Runtime.Providers.Unity.Data;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Providers.Unity.Wrappers
{
    internal sealed class RewardedAdWrapper : IDisposable
    {
        private readonly AdLoadRetryHandler _retryHandler;
        private readonly LevelPlayRewardedAd _rewardedAd;
        private readonly ReactiveProperty<bool> _isAvailable;
        private readonly Subject<Unit> _success;

        internal RewardedAdWrapper(
            string adUnitId,
            RetryConfig retryConfig,
            ReactiveProperty<bool> isAvailable,
            Subject<Unit> success)
        {
            _isAvailable = isAvailable;
            _success = success;
            _rewardedAd = new LevelPlayRewardedAd(adUnitId);
            _retryHandler = new AdLoadRetryHandler(Load, retryConfig);

            _rewardedAd.OnAdRewarded += OnRewardedAdRewarded;
            _rewardedAd.OnAdLoaded += OnLoaded;
            _rewardedAd.OnAdLoadFailed += OnLoadFailed;
            _rewardedAd.OnAdClosed += OnClosed;
        }

        internal void Show(string placement)
        {
            _rewardedAd.ShowAd(placement);
        }

        internal void Load()
        {
            _rewardedAd.LoadAd();
        }

        private void OnRewardedAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            _success.OnNext(Unit.Default);
            _isAvailable.OnNext(_rewardedAd.IsAdReady());
        }

        private void OnLoaded(LevelPlayAdInfo info)
        {
            _retryHandler.OnLoadSuccess();
            _isAvailable.OnNext(_rewardedAd.IsAdReady());
        }

        private void OnLoadFailed(LevelPlayAdError error)
        {
            _retryHandler.OnLoadFailed();
            _isAvailable.OnNext(_rewardedAd.IsAdReady());
        }

        private void OnClosed(LevelPlayAdInfo info)
        {
            _isAvailable.OnNext(_rewardedAd.IsAdReady());
            _rewardedAd.LoadAd();
        }

        public void Dispose()
        {
            _rewardedAd.OnAdRewarded -= OnRewardedAdRewarded;
            _rewardedAd.OnAdLoaded -= OnLoaded;
            _rewardedAd.OnAdLoadFailed -= OnLoadFailed;
            _rewardedAd.OnAdClosed -= OnClosed;
            _rewardedAd.Dispose();
            _retryHandler.Dispose();
        }
    }
}
#endif