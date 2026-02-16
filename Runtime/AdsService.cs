using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.Extensions.Observables;
using ImprovedTimers;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime
{
    [PublicAPI]
    public sealed class AdsService<TPlacement> : IAdsService<TPlacement>, IDisposable
        where TPlacement : unmanaged, Enum
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _adsProvider.IsRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _adsProvider.IsInterstitialAvailable;

        private Action _onRewarded;

        private CountdownTimer _interstitialTimer;

        private IDisposable _rewardSubscription;

        private readonly IAdsProvider<TPlacement> _adsProvider;
        private readonly AdsConfigBase<TPlacement> _adsConfig;
        private readonly AdsPreloader _rewardedPreloader;
        private readonly AdsPreloader _interstitialPreloader;

        public AdsService(IAdsProvider<TPlacement> adsProvider, AdsConfigBase<TPlacement> adsConfig)
        {
            _adsProvider = adsProvider;
            _adsConfig = adsConfig;

            _adsProvider.Initialize();

            _rewardedPreloader = new AdsPreloader(
                _adsProvider.IsRewardedAvailable,
                _adsProvider.PreloadRewarded,
                _adsConfig.MaxRewardedLoadAttempts,
                _adsConfig.RetryLoadDelay);

            _interstitialPreloader = new AdsPreloader(
                _adsProvider.IsInterstitialAvailable,
                _adsProvider.PreloadInterstitial,
                _adsConfig.MaxInterstitialLoadAttempts,
                _adsConfig.RetryLoadDelay);

            _interstitialTimer = new CountdownTimer(_adsConfig.TimeBetweenInterstitials);

            _rewardSubscription = _adsProvider.OnRewardedSuccess
                .SubscribeSelf(this, static self => self.ExecuteReward());
        }

        public void ShowRewardedAd(TPlacement placement, Action onRewarded)
        {
            if (!_adsProvider.IsInitialized || !_adsConfig.SupportedPlacements[placement])
                return;

            _onRewarded = onRewarded;
            _adsProvider.ShowRewarded(placement);
        }

        public void ShowInterstitial(TPlacement placement)
        {
            if (!_adsProvider.IsInitialized || !_adsConfig.SupportedPlacements[placement])
                return;

            if (_interstitialTimer.IsRunning)
                return;

            _adsProvider.ShowInterstitial();

            _interstitialTimer.Reset();
            _interstitialTimer.Start();
        }

        private void ExecuteReward()
        {
            if (!_adsProvider.IsInitialized)
                return;

            _onRewarded?.Invoke();
            _onRewarded = null;
        }

        public void Dispose()
        {
            _interstitialTimer.Dispose();
            _rewardSubscription.Dispose();
            _rewardedPreloader.Dispose();
            _interstitialPreloader.Dispose();

            _adsProvider.Dispose();
        }
    }
}