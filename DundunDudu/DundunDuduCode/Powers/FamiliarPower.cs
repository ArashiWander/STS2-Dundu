using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 熟悉 (Familiar) — 极限格斗 archetype resource (Counter, no decay). Gains +1 whenever the owner takes UNBLOCKED
/// damage (AfterDamageReceived, DamageResult.UnblockedDamage); grants +1 attack damage per stack via
/// ModifyDamageAdditive, doubled while 千锤百炼 (<see cref="TemperedPower"/>) is active. Numbers in unit-tested
/// <see cref="FamiliarMath"/>. MP-safe: per-creature, reads own state only, no static/RNG.
/// </summary>
public sealed class FamiliarPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int Stacks => (int)base.Amount;
    private bool IsTempered => base.Owner.GetPower<TemperedPower>() != null;

    // 受到未格挡伤害 → +1 熟悉 (this is what 以伤换伤's self-damage feeds).
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner || result.UnblockedDamage <= 0) return;

        MainFile.Logger.Info($"[熟悉] 受未格挡伤害 {result.UnblockedDamage} → +1 (当前 {Stacks} → {Stacks + 1})");
        await PowerCmd.Apply<FamiliarPower>(choiceContext, base.Owner, 1m, base.Owner, null);
    }

    // 攻击伤害 +1/层 (千锤百炼时 +2/层). Additive — applied before 闷气/绝境 multipliers.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner || !props.IsPoweredAttack()) return 0m;

        int bonus = FamiliarMath.Bonus(Stacks, IsTempered);
        if (bonus != 0)
        {
            MainFile.Logger.Info($"[熟悉] 攻击 +{bonus} (层={Stacks}, 千锤百炼={IsTempered}) → {target?.GetType().Name}");
        }
        return bonus;
    }
}
