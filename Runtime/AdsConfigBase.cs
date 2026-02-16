using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.CustomTypes.Collections;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace AdsIntegration.Runtime
{
    internal abstract class AdsConfigBase<TConfig, TPlacement> : SingletonScriptableObject<TConfig>, IAdsConfig<TPlacement>
        where TPlacement : unmanaged, Enum
        where TConfig : AdsConfigBase<TConfig, TPlacement>
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