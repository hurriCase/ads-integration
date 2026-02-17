using System;
using AdsIntegration.Runtime.Base;
using JetBrains.Annotations;

namespace AdsIntegration.Runtime.Providers.None
{
    [PublicAPI]
    public sealed class NoneConfig<TPlacement> : AdsConfigBase<TPlacement>
        where TPlacement : unmanaged, Enum { }
}