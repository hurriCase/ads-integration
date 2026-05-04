using System;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity.Data
{
    [Serializable]
    internal struct AdsData
    {
        [field: SerializeField] internal string AppKey { get; private set; }
        [field: SerializeField] internal string RewardedAdUnitId { get; private set; }
        [field: SerializeField] internal string InterstitialAdUnitId { get; private set; }
    }
}