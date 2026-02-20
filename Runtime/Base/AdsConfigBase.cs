using System;
using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEngine;

namespace AdsIntegration.Runtime.Base
{
    public abstract class AdsConfigBase<TPlacement> : ScriptableObject
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] public float TimeBetweenInterstitials { get; private set; } = 60f;
        [field: SerializeField] public EnumArray<TPlacement, bool> SupportedPlacements { get; private set; } = new();
    }
}