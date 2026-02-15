using CustomUtils.Runtime.AssetLoader;
using CustomUtils.Runtime.CustomTypes.Singletons;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity
{
    /// <inheritdoc />
    /// <summary>
    /// Scriptable object to store advertising service configuration data
    /// </summary>
    [Resource(FullSettingsPath, nameof(LevelPlayConfig), ResourceSettingsPath)]
    internal sealed class LevelPlayConfig : SingletonScriptableObject<LevelPlayConfig>
    {
        [field: SerializeField] internal string AppKey { get; private set; }
        [field: SerializeField] internal string RewardedAdUnitId { get; private set; }
        [field: SerializeField] internal string InterstitialAdUnitId { get; private set; }

        private const string FullSettingsPath = "Assets/Resources/" + ResourceSettingsPath;
        private const string ResourceSettingsPath = "AdsIntegration";
    }
}