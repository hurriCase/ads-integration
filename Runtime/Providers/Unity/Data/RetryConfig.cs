using System;
using UnityEngine;

namespace AdsIntegration.Runtime.Providers.Unity.Data
{
    [Serializable]
    internal struct RetryConfig
    {
        [field: SerializeField] internal int MaxAttempts { get; private set; }
        [field: SerializeField] internal float RetryDelay { get; private set; }
    }
}