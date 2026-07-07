using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using R3;
using UnityEngine;

#if PRIMETWEEN_INSTALLED
using PrimeTween;
#endif

namespace AdsIntegration.Runtime.Providers.None
{
    [PublicAPI]
    public sealed class NoneAdsProvider<TPlacement> : IAdsProvider<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;
        public Observable<Unit> OnInterstitialClosed => _interstitialClosed;
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private readonly Subject<Unit> _rewardedSuccess = new();
        private readonly Subject<Unit> _interstitialClosed = new();
        private readonly ReactiveProperty<bool> _isRewardedAvailable = new(true);
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new(true);

        private const float FakeAdsFinishDuration = 1f;

        public void Initialize()
        {
            IsInitialized = true;
        }

        public void ShowRewarded(TPlacement placement)
        {
#if PRIMETWEEN_INSTALLED
            Tween.Delay(FakeAdsFinishDuration, () => FinishRewardedAd(placement), useUnscaledTime: true);
#else
            FinishRewardedAd(placement);
#endif
        }

        private void FinishRewardedAd(TPlacement placement)
        {
            Debug.Log($"[NoneAdsProvider::ShowRewarded] Rewarded ads was shown for {placement}");
            _rewardedSuccess.OnNext(Unit.Default);
            _isRewardedAvailable.OnNext(false);
        }

        public void ShowInterstitial()
        {
            Debug.Log("[NoneAdsProvider::ShowInterstitial] Interstitial ads was shown");
            _isInterstitialAvailable.OnNext(false);
        }

        public void PreloadRewarded()
        {
            Debug.Log("[NoneAdsProvider::PreloadRewarded] Rewarded ads was preloaded");
            _isRewardedAvailable.OnNext(true);
        }

        public void PreloadInterstitial()
        {
            Debug.Log("[NoneAdsProvider::PreloadInterstitial] Interstitial ads was preloaded");
            _isInterstitialAvailable.OnNext(true);
        }

        public void Dispose()
        {
            _rewardedSuccess.Dispose();
            _interstitialClosed.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();
        }
    }
}