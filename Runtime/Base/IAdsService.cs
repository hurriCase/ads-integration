using System;
using JetBrains.Annotations;
using R3;

namespace AdsIntegration.Runtime.Base
{
    [PublicAPI]
    public interface IAdsService<in TPlacement>
    {
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        void Initialize();
        void ShowRewardedAd(TPlacement placement, Action onRewarded);
        void ShowInterstitial(TPlacement placement);
    }
}