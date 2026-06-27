using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 残影 (Afterimage) — 太空步 archetype resource (Counter). While stacks remain, an incoming attack hit (≥1) is
/// fully negated, consuming one stack per hit. Unused stacks clear at the enemy turn end (block rule — no carry).
/// Shape mirrors IntangiblePower (which caps to 1); we negate to 0. Numbers live in unit-tested
/// <see cref="AfterimageMath"/>. MP-safe: per-creature, no static/RNG.
///
/// Watch-item (Dundun SPEC §3): the per-hit consume on MULTI-hit attacks is verified in-game (VERIFY-QUEUE F).
/// </summary>
public sealed class AfterimageStacksPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int Stacks => (int)base.Amount;

    // 完全否定一次攻击: any hit ≥1 to the owner deals 0 HP loss while stacks remain.
    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress) return amount;
        if (target != base.Owner) return amount;
        return AfterimageMath.HpLostAfter(Stacks, amount);
    }

    // Fires after an actual HP-loss modification → consume exactly one stack for the negated hit.
    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (Stacks > 0)
        {
            MainFile.Logger.Info($"[残影] 否定一次攻击, 消耗 1 层 (剩 {Stacks - 1})");
            await PowerCmd.Decrement(this);
        }
    }

    // Preview / block-loss mirror: targeted attacks show as fully negated while stacks remain.
    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner) return decimal.MaxValue;
        return Stacks > 0 ? 0m : decimal.MaxValue;
    }

    // 不可跨回合囤积: clear all unused 残影 at the enemy turn end (same timing as block / IntangiblePower's tick).
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy || Stacks <= 0) return;
        MainFile.Logger.Info($"[残影] 回合末清空 {Stacks} 层");
        await PowerCmd.Remove(this);
    }
}
