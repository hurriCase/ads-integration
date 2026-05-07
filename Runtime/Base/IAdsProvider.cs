using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    public interface IAdsProvider<in TPlacement> : IDisposable
        where TPlacement : unmanaged, Enum
    {
        bool IsInitialized { get; }
        Observable<Unit> OnRewardedSuccess { get; }
        Observable<Unit> OnInterstitialClosed { get; }
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        void Initialize();
        void ShowRewarded(TPlacement placement);
        void ShowInterstitial();
        void PreloadRewarded();
        void PreloadInterstitial();
    }
}