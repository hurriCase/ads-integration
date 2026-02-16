#if CRAZY_GAMES
using System;
using AdsIntegration.Runtime.Base;
using CustomUtils.Runtime.AssetLoader;

namespace AdsIntegration.Runtime.Providers.Crazy
{
    [Resource(FullSettingsPath, nameof(CrazyGamesConfig<TPlacement>), ResourceSettingsPath)]
    public sealed class CrazyGamesConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum { }
}
#endif