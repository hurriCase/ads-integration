using System;
using CustomUtils.Runtime.AssetLoader;

namespace AdsIntegration.Runtime.Providers.None
{
    [Resource(FullSettingsPath, nameof(NoneConfig<TPlacement>), ResourceSettingsPath)]
    internal sealed class NoneConfig<TPlacement> : AdsConfigBase<NoneConfig<TPlacement>, TPlacement>
        where TPlacement : unmanaged, Enum { }
}