using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 千锤百炼 (Tempered) — a passive flag-power. While present, <see cref="FamiliarPower"/>'s attack bonus is
/// doubled (+2 per 熟悉 stack instead of +1). Single stack-type (presence, not count). MP-safe: per-creature flag.
/// </summary>
public sealed class TemperedPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
