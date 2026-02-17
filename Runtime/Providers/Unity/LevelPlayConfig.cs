using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity
{
    [PublicAPI]
    public class LevelPlayConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] internal string AppKey { get; private set; }
        [field: SerializeField] internal string RewardedAdUnitId { get; private set; }
        [field: SerializeField] internal string InterstitialAdUnitId { get; private set; }
    }
}