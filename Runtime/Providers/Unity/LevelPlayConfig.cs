using System;
using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity
{
    [Resource(FullSettingsPath, nameof(LevelPlayConfig<TPlacement>), ResourceSettingsPath)]
    internal sealed class LevelPlayConfig<TPlacement> : AdsConfigBase<LevelPlayConfig<TPlacement>, TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] internal string AppKey { get; private set; }
        [field: SerializeField] internal string RewardedAdUnitId { get; private set; }
        [field: SerializeField] internal string InterstitialAdUnitId { get; private set; }
    }
}