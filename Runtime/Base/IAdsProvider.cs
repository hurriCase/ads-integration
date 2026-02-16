using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    internal interface IAdsProvider<TPlacement> : IDisposable
        where TPlacement : unmanaged, Enum
    {
        bool IsInitialized { get; }
        Observable<Unit> OnRewardedSuccess { get; }
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        IAdsConfig<TPlacement> AdsConfig { get; }
        void Initialize();
        void ShowRewarded(TPlacement placement);
        void ShowInterstitial();
        void PreloadRewarded();
        void PreloadInterstitial();
    }
}