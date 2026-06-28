using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 无法攻击 (No-Attacks) — a one-turn Debuff: while present, the owner can't play attack cards (ShouldPlay → false
/// for their attacks). Removed at the owner's turn end. Applied by 银色山泉 as its cost. MP-safe (per-creature flag).
/// </summary>
public sealed class NoAttacksPower : CustomPowerModel
{
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner.Creature == base.Owner && card.Type == CardType.Attack) return false;
        return true;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner)) return;
        await PowerCmd.Remove(this);
    }
}
