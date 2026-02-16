#if AZERION && !UNITY_EDITOR
using System;
using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Azerion
{
    [Resource(FullSettingsPath, nameof(AzerionConfig<TPlacement>), ResourceSettingsPath)]
    internal sealed class AzerionConfig<TPlacement> : AdsConfigBase<AzerionConfig<TPlacement>, TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] internal string GameKey { get; private set; }
    }
}
#endif