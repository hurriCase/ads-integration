using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;

namespace AdsIntegration.Runtime.Providers.None
{
    [Resource(FullSettingsPath, nameof(NoneConfig<TPlacement>), ResourceSettingsPath)]
    internal sealed class NoneConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum { }
}