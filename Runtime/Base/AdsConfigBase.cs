using System;
 using CustomUtils.Runtime.CustomTypes.Collections;
using UnityEngine;

namespace AdsIntegration.Runtime.Base
{
    public abstract class AdsConfigBase<TPlacement> : ScriptableObject
        where TPlacement : unmanaged, Enum
    {
        [field: SerializeField] public float TimeBetweenInterstitials { get; private set; } = 60f;
        [field: SerializeField] public int MaxInterstitialLoadAttempts { get; private set; } = 3;
        [field: SerializeField] public int MaxRewardedLoadAttempts { get; private set; } = 3;
        [field: SerializeField] public float RetryLoadDelay { get; private set; } = 30f;

        [field: SerializeField] public EnumArray<TPlacement, bool> SupportedPlacements { get; private set; } = new();

        protected const string FullSettingsPath = "Assets/Resources/" + ResourceSettingsPath;
        protected const string ResourceSettingsPath = "AdsIntegration";
    }
}