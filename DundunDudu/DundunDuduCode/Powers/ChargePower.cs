using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 蓄力 (Charge) — 巨剑 archetype resource (Counter). If the owner ends a turn WITHOUT playing an attack, gains
/// +1 (checked via combat history, FeralPower pattern). Persists across turns (no decay); spender cards
/// (斩断一切) read the stack count and remove it. MP-safe: per-creature, per-owner history read, no static/RNG.
/// </summary>
public sealed class ChargePower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner)) return;
        if (CombatHistory.AttacksPlayedThisTurn(base.Owner, base.Owner.CombatState!) > 0) return;

        MainFile.Logger.Info($"[蓄力] 本回合未出攻击 → +1 (当前 {(int)base.Amount} → {(int)base.Amount + 1})");
        await PowerCmd.Apply<ChargePower>(choiceContext, base.Owner, 1m, base.Owner, null);
    }
}
