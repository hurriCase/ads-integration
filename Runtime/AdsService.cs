using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Providers;
using Cysharp.Threading.Tasks;
using ImprovedTimers;
using R3;

namespace AdsIntegration.Runtime
{
    public sealed class AdsService<TPlacement> : IAdsService<TPlacement>, IDisposable
        where TPlacement : unmanaged, Enum
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _adsProvider?.IsRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _adsProvider?.IsInterstitialAvailable;

        private Action _onRewarded;
        private int _rewardLoadAttemptCount;

        private CountdownTimer _interstitialTimer;
        private int _interstitialLoadAttemptCount;

        private IDisposable _preloadSubscriptions;

        private IAdsProvider<TPlacement> _adsProvider;
        private IAdsConfig<TPlacement> _adsConfig;

        public void Initialize()
        {
            _adsProvider =
#if GooglePlay
                new IronSourceAdService<TPlacement>();
#elif AZERION && !UNITY_EDITOR
                new AzerionAdsService<TPlacement>();
#elif CRAZY_GAMES && !BASIC_LAUNCH
                new CrazyGamesAdService<TPlacement>();
#else
                new NoneAdsProvider<TPlacement>();
#endif

            _adsProvider.Initialize();

            _adsConfig = _adsProvider.AdsConfig;

            _interstitialTimer = new CountdownTimer(_adsConfig.TimeBetweenInterstitials);

            SetupSubscriptions();
        }

        private void SetupSubscriptions()
        {
            var interstitialSubscription = _adsProvider.IsInterstitialAvailable
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Subscribe(this, static (isAvailable, self) => self.HandleInterstitialAvailabilityChange(isAvailable));

            var rewardedSubscription = _adsProvider.IsRewardedAvailable
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Subscribe(this, static (isAvailable, self) => self.HandleRewardedAvailabilityChange(isAvailable));

            var rewardSubscription = _adsProvider.OnRewardedSuccess
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Do(this, static (_, self) => self._onRewarded?.Invoke())
                .Subscribe(this, static (_, self) => self._onRewarded = null);

            _preloadSubscriptions =
                Disposable.Combine(interstitialSubscription, rewardedSubscription, rewardSubscription);
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

        private void HandleInterstitialAvailabilityChange(bool isAvailable)
        {
            if (isAvailable)
            {
                _interstitialLoadAttemptCount = 0;
                return;
            }

            PreloadInterstitialAsync().Forget();
        }

        private void HandleRewardedAvailabilityChange(bool isAvailable)
        {
            if (isAvailable)
            {
                _rewardLoadAttemptCount = 0;
                return;
            }

            PreloadRewardedAsync().Forget();
        }

        private async UniTask PreloadRewardedAsync()
        {
            if (!_adsProvider.IsInitialized || _adsProvider.IsRewardedAvailable.CurrentValue)
                return;

            if (_rewardLoadAttemptCount >= _adsConfig.MaxRewardedLoadAttempts)
                return;

            await UniTask.WaitForSeconds(_adsConfig.RetryLoadDelay);

            _adsProvider.PreloadRewarded();

            _rewardLoadAttemptCount++;
        }

        private async UniTask PreloadInterstitialAsync()
        {
            if (!_adsProvider.IsInitialized || _adsProvider.IsInterstitialAvailable.CurrentValue)
                return;

            if (_interstitialLoadAttemptCount >= _adsConfig.MaxInterstitialLoadAttempts)
                return;

            await UniTask.WaitForSeconds(_adsConfig.RetryLoadDelay);

            _adsProvider.PreloadInterstitial();

            _interstitialLoadAttemptCount++;
        }

        public void Dispose()
        {
            _interstitialTimer.Dispose();
            _preloadSubscriptions.Dispose();

            _adsProvider.Dispose();
        }
    }
}