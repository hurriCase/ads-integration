using System;
using CustomUtils.Runtime.AssetLoader;

namespace AdsIntegration.Runtime.Providers.Crazy
{
    [Resource(FullSettingsPath, nameof(CrazyGamesConfig<TPlacement>), ResourceSettingsPath)]
    internal sealed class CrazyGamesConfig<TPlacement> : AdsConfigBase<CrazyGamesConfig<TPlacement>, TPlacement>
        where TPlacement : unmanaged, Enum { }
}