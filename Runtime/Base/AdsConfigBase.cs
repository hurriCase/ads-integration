using System;
using CustomUtils.Collections.Scripts;
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