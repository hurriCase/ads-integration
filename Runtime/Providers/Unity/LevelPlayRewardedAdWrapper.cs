using System;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Providers.Unity
{
    internal sealed class RewardedAdWrapper : IDisposable
    {
        private readonly LevelPlayRewardedAd _rewardedAd;
        private readonly ReactiveProperty<bool> _isAvailable;
        private readonly Subject<Unit> _success;

        internal RewardedAdWrapper(string adUnitId, ReactiveProperty<bool> isAvailable, Subject<Unit> success)
        {
            _isAvailable = isAvailable;
            _success = success;
            _rewardedAd = new LevelPlayRewardedAd(adUnitId);

            _rewardedAd.OnAdRewarded += OnRewardedAdRewarded;
            _rewardedAd.OnAdLoaded += OnLoaded;
            _rewardedAd.OnAdLoadFailed += OnLoadFailed;
            _rewardedAd.OnAdClosed += OnClosed;
        }

        internal void Show(string placement) => _rewardedAd.ShowAd(placement);
        internal void Load() => _rewardedAd.LoadAd();

        private void OnRewardedAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            _success.OnNext(Unit.Default);
            _isAvailable.OnNext(false);
        }

        private void OnLoaded(LevelPlayAdInfo info)
        {
            _isAvailable.OnNext(true);
        }

        private void OnLoadFailed(LevelPlayAdError error)
        {
            _isAvailable.OnNext(false);
        }

        private void OnClosed(LevelPlayAdInfo info)
        {
            _isAvailable.OnNext(false);
        }

        public void Dispose()
        {
            _rewardedAd.OnAdRewarded -= OnRewardedAdRewarded;
            _rewardedAd.OnAdLoaded -= OnLoaded;
            _rewardedAd.OnAdLoadFailed -= OnLoadFailed;
            _rewardedAd.OnAdClosed -= OnClosed;
            _rewardedAd.Dispose();
        }
    }
}