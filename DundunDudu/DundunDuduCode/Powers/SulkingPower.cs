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
/// 闷气 (Sulking) — 墩墩's core resource, 0–10 stacks. All numeric rules live in the unit-tested
/// <see cref="SulkingMath"/>; this class only wires those formulas into PowerModel hooks + diagnostic logging.
///
/// Tiers (墩墩角色Mod设计文档1.md §2.1):
///   3–5 警觉 ×1.5 attack · 6–8 爆发 ×2.0 + 免疫负面 · 9 临界 维持 + 回合始受1穿透 · 10 彻底自闭 + 回合末受3穿透.
///   (10 封技能 via ShouldPlay is deferred — not in this phase's scope.)
///
/// MP-safe: a per-creature PowerModel whose Amount rides the synced combat state; no static counters, no RNG;
/// 闷气 is personal so it does NOT scale with player count (ShouldScaleInMultiplayer left default false).
/// Gains flow through <see cref="Sulking.Gain"/> (clamps to ≤10); AfterApplied is a封顶 backstop.
/// </summary>
public sealed class SulkingPower : CustomPowerModel
{
    // Placeholder icons (P1a) — Custom*IconPath overrides, not ICustomPower (those collide with PowerModel.BigIcon).
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Current stacks, clamped to the design's [0,10] band.</summary>
    private int Stacks => SulkingMath.Cap((int)base.Amount);

    // 分层改伤: scale ONLY 墩墩's outgoing powered attacks (KnockdownPower direction). 3–5 ×1.5, 6–10 ×2.0.
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }

        decimal mult = SulkingMath.TierMultiplier(Stacks);
        if (mult != 1m)
        {
            MainFile.Logger.Info($"[闷气] 改伤 stacks={Stacks} ×{mult} → {target?.GetType().Name}");
        }
        return mult;
    }

    // 免疫负面 (6–9): negate incoming visible debuffs (ArtifactPower pattern, but continuous — no charge spent).
    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? applier, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (!SulkingMath.IsDebuffImmune(Stacks)) return false;
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff) return false;
        if (!canonicalPower.IsVisible) return false;

        modifiedAmount = 0m;
        MainFile.Logger.Info($"[闷气] 免疫负面 stacks={Stacks} 挡下 {canonicalPower.GetType().Name} (amt={amount})");
        return true;
    }

    // 临界期(9): take 1 穿透 (Unblockable) self-damage at TURN START.
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner)) return;
        int dmg = SulkingMath.CriticalSelfDamage(Stacks);
        if (dmg <= 0) return;

        MainFile.Logger.Info($"[闷气] 临界自伤 stacks={Stacks} 墩墩受 {dmg} 穿透 (回合始)");
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.Owner, dmg, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }

    // 彻底自闭(10): take 3 穿透 (Unblockable) self-damage at TURN END.
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (!participants.Contains(base.Owner)) return;
        int dmg = SulkingMath.MeltdownSelfDamage(Stacks);
        if (dmg <= 0) return;

        MainFile.Logger.Info($"[闷气] 自闭自伤 stacks={Stacks} 墩墩受 {dmg} 穿透 (回合末)");
        await CreatureCmd.Damage(choiceContext, base.Owner, dmg, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }

    // 层数封顶 backstop: gains normally route through Sulking.Gain (pre-clamped), but if anything applies 闷气
    // directly past 10, snap it back. Uses the public SetAmount; silent so it doesn't re-fire vfx.
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        await base.AfterApplied(applier, cardSource);
        if ((int)base.Amount > SulkingMath.MaxStacks)
        {
            MainFile.Logger.Info($"[闷气] 封顶 {(int)base.Amount}→{SulkingMath.MaxStacks}");
            base.SetAmount(SulkingMath.MaxStacks, silent: true);
        }
    }
}
