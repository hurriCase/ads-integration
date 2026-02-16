#if LEVEL_PLAY
using System;
using R3;
using Unity.Services.LevelPlay;

namespace AdsIntegration.Runtime.Providers.Unity
{
    internal sealed class InterstitialAdWrapper : IDisposable
    {
        private readonly LevelPlayInterstitialAd _ad;
        private readonly ReactiveProperty<bool> _isAvailable;

        internal InterstitialAdWrapper(string adUnitId, ReactiveProperty<bool> isAvailable)
        {
            _isAvailable = isAvailable;
            _ad = new LevelPlayInterstitialAd(adUnitId);

            _ad.OnAdLoaded += OnLoaded;
            _ad.OnAdLoadFailed += OnLoadFailed;
            _ad.OnAdClosed += OnClosed;
        }

        internal void Show() => _ad.ShowAd();
        internal void Load() => _ad.LoadAd();

        private void OnLoaded(LevelPlayAdInfo info) => _isAvailable.OnNext(true);
        private void OnLoadFailed(LevelPlayAdError error) => _isAvailable.OnNext(false);
        private void OnClosed(LevelPlayAdInfo info) => _isAvailable.OnNext(false);

        public void Dispose()
        {
            _ad.OnAdLoaded -= OnLoaded;
            _ad.OnAdLoadFailed -= OnLoadFailed;
            _ad.OnAdClosed -= OnClosed;
            _ad.Dispose();
        }
    }
}
#endif