using System;
using System.Diagnostics;
using AdsIntegration.Runtime.Base;
using R3;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity
{
    internal sealed class LevelPlayAdsProvider : IAdsProvider
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;

        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private LevelPlayConfig LevelPlayConfig => LevelPlayConfig.Instance;

        private readonly Subject<Unit> _rewardedSuccess = new();

        private readonly ReactiveProperty<bool> _isRewardedAvailable = new();
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new();

        private readonly IAdImpressionTracker _adImpressionTracker;

        private LevelPlayRewardedAd _rewardedAd;
        private LevelPlayInterstitialAd _interstitialAd;

        internal LevelPlayAdsProvider(IAdImpressionTracker adImpressionTracker)
        {
            _adImpressionTracker = adImpressionTracker;
        }

        public void Initialize()
        {
            LevelPlay.OnInitSuccess += OnSdkInitSuccess;
            LevelPlay.OnInitFailed += OnSdkInitFailed;
            LevelPlay.OnImpressionDataReady += OnImpressionDataReady;

            LevelPlay.Init(LevelPlayConfig.AppKey);
        }

        public void ShowRewarded(Enum placement)
        {
            var placementName = placement.ToString();

            _rewardedAd.ShowAd(placementName);
        }

        private void OnSdkInitSuccess(LevelPlayConfiguration levelPlayConfiguration)
        {
            _rewardedAd = new LevelPlayRewardedAd(LevelPlayConfig.RewardedAdUnitId);

            _rewardedAd.OnAdRewarded += OnAdRewarded;
            _rewardedAd.OnAdLoaded += OnRewardedAdLoaded;
            _rewardedAd.OnAdLoadFailed += OnRewardedAdLoadFailed;
            _rewardedAd.OnAdClosed += OnRewardedAdClosed;

            _interstitialAd = new LevelPlayInterstitialAd(LevelPlayConfig.InterstitialAdUnitId);

            _interstitialAd.OnAdLoaded += OnInterstitialAdLoaded;
            _interstitialAd.OnAdLoadFailed += OnInterstitialAdLoadFailed;
            _interstitialAd.OnAdClosed += OnInterstitialAdClosed;

            Application.focusChanged += OnApplicationFocusChanged;

            EnableTestMode();

            IsInitialized = true;
        }

        private void OnSdkInitFailed(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;
        }

        public bool IsRewardedAdAvailable() => _rewardedAd.IsAdReady();

        public void PreloadRewarded()
        {
            _rewardedAd.LoadAd();
        }

        private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
        {
            _isRewardedAvailable.OnNext(true);
        }

        private void OnRewardedAdLoadFailed(LevelPlayAdError adError)
        {
            _isRewardedAvailable.OnNext(false);
        }

        private void OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            _rewardedSuccess.OnNext(Unit.Default);
            _isRewardedAvailable.OnNext(false);
        }

        private void OnRewardedAdClosed(LevelPlayAdInfo adInfo)
        {
            _isRewardedAvailable.OnNext(false);
        }

        public void ShowInterstitial()
        {
            _interstitialAd.ShowAd();
        }

        private bool IsInterstitialAdReady() => _interstitialAd.IsAdReady();

        public void PreloadInterstitial()
        {
            _interstitialAd.LoadAd();
        }

        private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
        {
            _isInterstitialAvailable.OnNext(true);
        }

        private void OnInterstitialAdLoadFailed(LevelPlayAdError adError)
        {
            _isInterstitialAvailable.OnNext(false);
        }

        private void OnInterstitialAdClosed(LevelPlayAdInfo adInfo)
        {
            _isInterstitialAvailable.OnNext(false);
        }

        private void OnImpressionDataReady(LevelPlayImpressionData impressionData)
        {
            if (impressionData == null)
                return;

            _adImpressionTracker?.TrackAdImpression(impressionData);
        }

        [Conditional("ADS_TEST_MODE")]
        private void EnableTestMode()
        {
            Logger.Log("[LevelPlayAdsProvider::EnableTestMode] Launching test suite");

            LevelPlay.LaunchTestSuite();
        }

        private void OnApplicationFocusChanged(bool hasFocus)
        {
            LevelPlay.SetPauseGame(!hasFocus);
        }

        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= OnSdkInitSuccess;
            LevelPlay.OnInitFailed -= OnSdkInitFailed;
            LevelPlay.OnImpressionDataReady -= OnImpressionDataReady;

            if (_rewardedAd != null)
            {
                _rewardedAd.OnAdRewarded -= OnAdRewarded;
                _rewardedAd.OnAdLoaded -= OnRewardedAdLoaded;
                _rewardedAd.OnAdLoadFailed -= OnRewardedAdLoadFailed;
                _rewardedAd.OnAdClosed -= OnRewardedAdClosed;
                _rewardedAd.Dispose();
            }

            _rewardedSuccess.Dispose();
            _isRewardedAvailable.Dispose();

            if (_interstitialAd != null)
            {
                _interstitialAd.OnAdLoaded -= OnInterstitialAdLoaded;
                _interstitialAd.OnAdLoadFailed -= OnInterstitialAdLoadFailed;
                _interstitialAd.OnAdClosed -= OnInterstitialAdClosed;
                _interstitialAd.Dispose();
            }

            _isInterstitialAvailable.Dispose();

            _adImpressionTracker.Dispose();

            Application.focusChanged -= OnApplicationFocusChanged;
        }
    }
}