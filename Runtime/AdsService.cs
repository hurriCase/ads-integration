using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.Extensions.Observables;
using ImprovedTimers;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime
{
    [PublicAPI]
    public class AdsService<TPlacement> : IAdsService<TPlacement>, IDisposable
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
            if (!ValidateRequest(placement, nameof(ShowRewardedAd)))
                return;

            Logger.Log($"[AdsService::ShowRewardedAd] Showing rewarded ad for {placement}");
            _onRewarded = onRewarded;
            _adsProvider.ShowRewarded(placement);
        }

        public void ShowInterstitial(TPlacement placement)
        {
            if (!ValidateRequest(placement, nameof(ShowInterstitial)))
                return;

            if (_interstitialTimer.IsRunning)
            {
                Logger.LogWarning("[AdsService::ShowInterstitial] Cooldown is active, skipping");
                return;
            }

            Logger.Log($"[AdsService::ShowInterstitial] Showing interstitial for {placement}");
            _adsProvider.ShowInterstitial();

            _interstitialTimer.Reset();
            _interstitialTimer.Start();
        }

        private void ExecuteReward()
        {
            if (!_adsProvider.IsInitialized)
            {
                Logger.LogWarning("[AdsService::ExecuteReward] Provider is not initialized, skipping reward");
                return;
            }

            Logger.Log("[AdsService::ExecuteReward] Reward executed successfully");
            _onRewarded?.Invoke();
            _onRewarded = null;
        }

        private bool ValidateRequest(TPlacement placement, string callerName)
        {
            if (!_adsProvider.IsInitialized)
            {
                Logger.LogWarning($"[AdsService::{callerName}] Provider is not initialized, skipping {placement}");
                return false;
            }

            if (_adsConfig.SupportedPlacements[placement])
                return true;

            Logger.LogWarning($"[AdsService::{callerName}] Placement {placement} is not supported");
            return false;
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