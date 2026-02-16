using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    public interface IAdsService<in TPlacement>
    {
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        void Initialize();
        void ShowRewardedAd(TPlacement placement, Action onRewarded);
        void ShowInterstitial(TPlacement placement);
    }
}