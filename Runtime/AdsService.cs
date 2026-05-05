using System;
using System.Runtime.CompilerServices;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.Extensions.Observables;
using ImprovedTimers;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime
{
    [PublicAPI]
    public class AdsService<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _adsProvider.IsRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _adsProvider.IsInterstitialAvailable;

        private Action _onRewarded;

        private readonly IAdsProvider<TPlacement> _adsProvider;
        private readonly AdsConfigBase<TPlacement> _adsConfig;

        private readonly CountdownTimer _interstitialTimer;
        private readonly IDisposable _subscriptions;

        public AdsService(IAdsProvider<TPlacement> adsProvider, AdsConfigBase<TPlacement> adsConfig)
        {
            _adsProvider = adsProvider;
            _adsConfig = adsConfig;

            _adsProvider.Initialize();

            _interstitialTimer = new CountdownTimer(_adsConfig.TimeBetweenInterstitials);

            _subscriptions = _adsProvider.OnRewardedSuccess
                .SubscribeSelf(this, static self => self.ExecuteReward());
        }

        public void ShowRewardedAd(TPlacement placement, Action onRewarded)
        {
            if (!ValidateRequest(placement))
                return;

            Logger.Log($"[AdsService::ShowRewardedAd] Showing rewarded ad for {placement}");
            _onRewarded = onRewarded;
            _adsProvider.ShowRewarded(placement);
        }

        public void ShowInterstitial(TPlacement placement)
        {
            if (!ValidateRequest(placement))
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
            Logger.Log("[AdsService::ExecuteReward] Reward executed successfully");
            _onRewarded?.Invoke();
            _onRewarded = null;
        }

        private bool ValidateRequest(TPlacement placement, [CallerMemberName] string callerName = "")
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
            _subscriptions.Dispose();

            _adsProvider.Dispose();
        }
    }
}