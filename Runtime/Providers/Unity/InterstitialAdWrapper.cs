#if LEVEL_PLAY
using System;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Providers.Unity
{
    internal sealed class InterstitialAdWrapper : IDisposable
    {
        private readonly LevelPlayInterstitialAd _interstitialAd;
        private readonly ReactiveProperty<bool> _isAvailable;

        internal InterstitialAdWrapper(string adUnitId, ReactiveProperty<bool> isAvailable)
        {
            _isAvailable = isAvailable;
            _interstitialAd = new LevelPlayInterstitialAd(adUnitId);

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

        private void OnLoaded(LevelPlayAdInfo info) => _isAvailable.OnNext(_interstitialAd.IsAdReady());
        private void OnLoadFailed(LevelPlayAdError error) => _isAvailable.OnNext(_interstitialAd.IsAdReady());
        private void OnClosed(LevelPlayAdInfo info) => _isAvailable.OnNext(_interstitialAd.IsAdReady());

        public void Dispose()
        {
            _interstitialAd.OnAdLoaded -= OnLoaded;
            _interstitialAd.OnAdLoadFailed -= OnLoadFailed;
            _interstitialAd.OnAdClosed -= OnClosed;
            _interstitialAd.Dispose();
        }
    }
}
#endif