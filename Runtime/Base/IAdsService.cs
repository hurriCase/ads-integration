using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    public interface IAdsService
    {
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        void Initialize();
        void ShowRewardedAd(Enum placement, Action onRewarded);
        void ShowInterstitial();
    }
}