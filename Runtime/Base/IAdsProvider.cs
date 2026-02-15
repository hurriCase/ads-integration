using System;
using R3;

namespace AdsIntegration.Runtime.Base
{
    internal interface IAdsProvider : IDisposable
    {
        bool IsInitialized { get; }
        Observable<Unit> OnRewardedSuccess { get; }
        ReadOnlyReactiveProperty<bool> IsRewardedAvailable { get; }
        ReadOnlyReactiveProperty<bool> IsInterstitialAvailable { get; }
        void Initialize();
        void ShowRewarded(Enum placement);
        void ShowInterstitial();
        void PreloadRewarded();
        void PreloadInterstitial();
    }
}