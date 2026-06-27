using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 绝境坚持 (Last Stand) — a passive flag-power: while the owner's HP is strictly below 30% of max, their attack
/// damage is ×1.3. Threshold + multiplier live in unit-tested <see cref="LastStandMath"/>. Single stack-type
/// (presence, not count). MP-safe: per-creature, reads own HP only, no static/RNG.
/// </summary>
public sealed class LastStandPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner || !props.IsPoweredAttack()) return 1m;
        decimal mult = LastStandMath.DamageMultiplier(base.Owner.CurrentHp, base.Owner.MaxHp);
        if (mult != 1m)
        {
            MainFile.Logger.Info($"[绝境坚持] HP {base.Owner.CurrentHp}/{base.Owner.MaxHp} (<30%) → 攻击 ×{mult} → {target?.GetType().Name}");
        }
        return mult;
    }
}
