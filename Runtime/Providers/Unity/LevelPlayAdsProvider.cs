using System;
using System.Diagnostics;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using R3;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity
{
    [PublicAPI]
    public sealed class LevelPlayAdsProvider<TPlacement> : IAdsProvider<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        public bool IsInitialized { get; private set; }

        public Observable<Unit> OnRewardedSuccess => _rewardedSuccess;
        public ReadOnlyReactiveProperty<bool> IsRewardedAvailable => _isRewardedAvailable;
        public ReadOnlyReactiveProperty<bool> IsInterstitialAvailable => _isInterstitialAvailable;

        private readonly Subject<Unit> _rewardedSuccess = new();
        private readonly ReactiveProperty<bool> _isRewardedAvailable = new();
        private readonly ReactiveProperty<bool> _isInterstitialAvailable = new();

        private RewardedAdWrapper _rewardedAd;
        private InterstitialAdWrapper _interstitialAd;

        private readonly IAdImpressionTracker _adImpressionTracker;
        private readonly LevelPlayConfig<TPlacement> _levelPlayConfig;

        public LevelPlayAdsProvider(LevelPlayConfig<TPlacement> levelPlayConfig, IAdImpressionTracker adImpressionTracker)
        {
            _levelPlayConfig = levelPlayConfig;
            _adImpressionTracker = adImpressionTracker;
        }

        public void Initialize()
        {
            LevelPlay.OnInitSuccess += OnSdkInitSuccess;
            LevelPlay.OnInitFailed += OnSdkInitFailed;
            LevelPlay.OnImpressionDataReady += OnImpressionDataReady;

            LevelPlay.Init(_levelPlayConfig.AppKey);
        }

        private void OnSdkInitSuccess(LevelPlayConfiguration levelPlayConfiguration)
        {
            _rewardedAd = new RewardedAdWrapper(_levelPlayConfig.RewardedAdUnitId, _isRewardedAvailable, _rewardedSuccess);
            _interstitialAd = new InterstitialAdWrapper(_levelPlayConfig.InterstitialAdUnitId, _isInterstitialAvailable);

            Application.focusChanged += OnApplicationFocusChanged;

            EnableTestMode();

            IsInitialized = true;
        }

        private void OnSdkInitFailed(LevelPlayInitError levelPlayInitError)
        {
            IsInitialized = false;
        }

        public void ShowRewarded(TPlacement placement)
        {
            _rewardedAd.Show(placement.ToString());
        }

        public void ShowInterstitial()
        {
            _interstitialAd.Show();
        }

        public void PreloadRewarded()
        {
            if (!IsInitialized)
                return;

            _rewardedAd.Load();
        }

        public void PreloadInterstitial()
        {
            if (!IsInitialized)
                return;

            _interstitialAd.Load();
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

            _rewardedAd?.Dispose();
            _interstitialAd?.Dispose();

            _rewardedSuccess.Dispose();
            _isRewardedAvailable.Dispose();
            _isInterstitialAvailable.Dispose();

            Application.focusChanged -= OnApplicationFocusChanged;
        }
    }
}