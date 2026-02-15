using System;
using AdsIntegration.Runtime.Base;
using AdsIntegration.Runtime.Providers;
using Cysharp.Threading.Tasks;
using ImprovedTimers;
using R3;

namespace AdsIntegration.Runtime
{
    public sealed class AdsService : IAdsService, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _adsProvider?.IsRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _adsProvider?.IsInterstitialAvailable;

        private Action _onRewarded;
        private int _rewardLoadAttemptCount;

        private CountdownTimer _interstitialTimer;
        private int _interstitialLoadAttemptCount;

        private IDisposable _preloadSubscriptions;

        private IAdsProvider _adsProvider;
        private AdsConfig _adsConfig;

        public void Initialize()
        {
            _adsProvider =
#if GooglePlay
                new IronSourceAdService();
#elif AZERION && !UNITY_EDITOR
                new AzerionAdsService();
#elif CRAZY_GAMES && !BASIC_LAUNCH
                new CrazyGamesAdService();
#else
                new NoneAdsProvider();
#endif

            _adsConfig = AdsConfig.Instance;
            var interstitialDelay = AdsConfig.Instance.TimeBetweenInterstitials;
            _interstitialTimer = new CountdownTimer(interstitialDelay);

            _adsProvider.Initialize();

            var interstitialSubscription = _adsProvider.IsInterstitialAvailable
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Subscribe(this, static (isAvailable, self) => self.HandleInterstitialAvailabilityChange(isAvailable));

            var rewardedSubscription = _adsProvider.IsRewardedAvailable
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Subscribe(this, static (isAvailable, self) => self.HandleRewardedAvailabilityChange(isAvailable));

            var rewardSubscription = _adsProvider.OnRewardedSuccess
                .Where(this, static (_, self) => self._adsProvider.IsInitialized)
                .Do(this, static (_, self) => self._onRewarded?.Invoke())
                .Subscribe(this, static (_, self) => self.PreloadRewardedAsync().Forget());

            _preloadSubscriptions =
                Disposable.Combine(interstitialSubscription, rewardedSubscription, rewardSubscription);
        }

        public void ShowRewardedAd(Enum placement, Action onRewarded)
        {
            if (!_adsProvider.IsInitialized)
                return;

            _onRewarded = onRewarded;
            _adsProvider.ShowRewarded(placement);
        }

        public void ShowInterstitial()
        {
            if (!_adsProvider.IsInitialized)
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