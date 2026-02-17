using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;

namespace AdsIntegration.Runtime.Providers.Crazy
{
    [PublicAPI]
    public class CrazyGamesConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum { }
}