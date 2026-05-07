#if LEVEL_PLAY
using System;
using AdsIntegration.Runtime.Providers.Unity.Data;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Providers.Unity.Wrappers
{
    internal sealed class InterstitialAdWrapper : IDisposable
    {
        private readonly AdLoadRetryHandler _retryHandler;
        private readonly LevelPlayInterstitialAd _interstitialAd;
        private readonly ReactiveProperty<bool> _isAvailable;
        private readonly Subject<Unit> _interstitialClosed;

        internal InterstitialAdWrapper(
            string adUnitId,
            RetryConfig retryConfig,
            ReactiveProperty<bool> isAvailable,
            Subject<Unit> interstitialClosed)
        {
            _isAvailable = isAvailable;
            _interstitialAd = new LevelPlayInterstitialAd(adUnitId);
            _retryHandler = new AdLoadRetryHandler(Load, retryConfig);
            _interstitialClosed = interstitialClosed;

            _interstitialAd.OnAdLoaded += OnLoaded;
            _interstitialAd.OnAdLoadFailed += OnLoadFailed;
            _interstitialAd.OnAdClosed += OnClosed;
        }

        internal void Show()
        {
            _interstitialAd.ShowAd();
        }

        internal void Load()
        {
            _interstitialAd.LoadAd();
        }

        private void OnLoaded(LevelPlayAdInfo info)
        {
            _retryHandler.OnLoadSuccess();
            _isAvailable.OnNext(_interstitialAd.IsAdReady());
        }

        private void OnLoadFailed(LevelPlayAdError error)
        {
            _retryHandler.OnLoadFailed();
            _isAvailable.OnNext(_interstitialAd.IsAdReady());
        }

        private void OnClosed(LevelPlayAdInfo info)
        {
            _isAvailable.OnNext(_interstitialAd.IsAdReady());
            _interstitialAd.LoadAd();
            _interstitialClosed.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _interstitialAd.OnAdLoaded -= OnLoaded;
            _interstitialAd.OnAdLoadFailed -= OnLoadFailed;
            _interstitialAd.OnAdClosed -= OnClosed;
            _interstitialAd.Dispose();
            _retryHandler.Dispose();
        }
    }
}
#endif