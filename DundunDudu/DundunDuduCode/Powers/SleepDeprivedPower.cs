using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 熬大夜 (Sleep-Deprived) — a Debuff (Counter). The turn's FIRST block card has its block reduced by the stack
/// count (floored at 0); decays −1 at the owner's turn end; while ≥3, the owner gains +1 闷气 at turn start (the
/// 闷气 link). Numbers live in unit-tested <see cref="SleepDeprivedMath"/>.
///
/// "First block card this turn" is derived from combat history (no synced per-turn flag) → MP-safe. Note: because
/// it's a Debuff, 闷气's 免疫负面 (6–9) will negate attempts to apply 熬大夜 while in that band — by design.
/// </summary>
public sealed class SleepDeprivedPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int Stacks => (int)base.Amount;

    // 每层：本回合「下一张」(= 第一张) 格挡牌效果 −X（最低0）。用 history 判首张，避免同步状态。
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != base.Owner || block <= 0m) return 0m;
        if (CombatHistory.BlockCardsPlayedThisTurn(base.Owner, base.Owner.CombatState!) != 1) return 0m;

        int reduce = SleepDeprivedMath.BlockReduction(Stacks, (int)block);
        if (reduce > 0)
        {
            MainFile.Logger.Info($"[熬大夜] stacks={Stacks} 削减本回合首张格挡牌 -{reduce}");
        }
        return -reduce;
    }

    // 闷气联动：≥3 层时回合开始 +1 闷气。
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(base.Owner) || !SleepDeprivedMath.GivesSulking(Stacks)) return;

        MainFile.Logger.Info($"[熬大夜] stacks={Stacks} ≥3 → 回合始 +1 闷气");
        await Sulking.Gain(new ThrowingPlayerChoiceContext(), base.Owner, 1, null);
    }

    // 衰减：owner 回合末 −1 层（与其他负面状态一致）。
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner)) return;

        MainFile.Logger.Info($"[熬大夜] 回合末衰减 -1 (从 {Stacks})");
        await PowerCmd.Decrement(this);
    }
}
