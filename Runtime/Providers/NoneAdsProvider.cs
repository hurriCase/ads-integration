using System;
using AdsIntegration.Runtime.Base;
using PrimeTween;
using R3;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers
{
    internal sealed class NoneAdsProvider : IAdsProvider
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private readonly Subject<Unit> _rewardedSuccess = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        private const float FakeAdsFinishDuration = 1f;

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void ShowRewarded(Enum placement)
        {
            Tween.Delay(FakeAdsFinishDuration, () =>
            {
                Debug.Log($"[NoneAdsProvider::ShowRewarded] Rewarded ads was shown for {placement}");
                _rewardedSuccess.OnNext(Unit.Default);
            }, useUnscaledTime: true);
        }

        public void ShowInterstitial()
        {
            Debug.Log("[NoneAdsProvider::ShowInterstitial] Interstitial ads was shown");
        }

        public void PreloadRewarded()
        {
            Debug.Log("[NoneAdsProvider::PreloadRewarded] Rewarded ads was preloaded");
        }

        public void PreloadInterstitial()
        {
            Debug.Log("[NoneAdsProvider::PreloadInterstitial] Interstitial ads was preloaded");
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();
        }
    }
}