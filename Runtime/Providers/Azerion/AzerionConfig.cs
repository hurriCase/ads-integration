#if AZERION
using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Azerion
{
    [Resource(FullSettingsPath, nameof(AzerionConfig<TPlacement>), ResourceSettingsPath)]
    public sealed class AzerionConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] internal string GameKey { get; private set; }
    }
}
#endif