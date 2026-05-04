using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity.Data
{
    [PublicAPI]
    public class LevelPlayConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] private AdsData _androidAdsData;
        [field: SerializeField] private AdsData _iosAdsData;

        [field: SerializeField] internal RetryConfig InterstitialConfig { get; private set; }
        [field: SerializeField] internal RetryConfig RewardedConfig { get; private set; }

        internal AdsData AdsData =>
#if UNITY_ANDROID
            _androidAdsData;
#elif UNITY_IOS
            _iosAdsData;
#else
            new();
#endif
    }
}