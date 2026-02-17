#if AZERION
using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Azerion
{
    [PublicAPI]
    public class AzerionConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] internal string GameKey { get; private set; }
    }
}
#endif