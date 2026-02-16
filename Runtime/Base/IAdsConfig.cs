using System;
using CustomUtils.Runtime.CustomTypes.Collections;

namespace AdsIntegration.Runtime.Base
{
    internal interface IAdsConfig<TPlacement> where TPlacement : unmanaged, Enum
    {
        float TimeBetweenInterstitials { get; }
        int MaxInterstitialLoadAttempts { get; }
        int MaxRewardedLoadAttempts { get; }
        float RetryLoadDelay { get; }
        EnumArray<TPlacement, bool> SupportedPlacements { get; }
    }
}