using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace AdsIntegration.Runtime
{
    [Resource(FullSettingsPath, nameof(AdsConfig), ResourceSettingsPath)]
    internal sealed class AdsConfig : SingletonScriptableObject<AdsConfig>
    {
        [field: SerializeField] internal float TimeBetweenInterstitials { get; private set; } = 60f;
        [field: SerializeField] internal int MaxInterstitialLoadAttempts { get; private set; } = 3;
        [field: SerializeField] internal int MaxRewardedLoadAttempts { get; private set; } = 3;
        [field: SerializeField] internal float RetryLoadDelay { get; private set; } = 30f;

        private const string FullSettingsPath = "Assets/Resources/" + ResourceSettingsPath;
        private const string ResourceSettingsPath = "AdsIntegration";
    }
}