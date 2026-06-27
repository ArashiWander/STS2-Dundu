using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// P1a TEST card (TEMP): add 3 闷气 to 墩墩. Play once → 警觉(3–5); twice → 爆发(6). Then play an attack
/// (稳扎一拳 7 → 10 / 14; 二段蹬腿 14 → 21 / 28) and watch the damage scale ×1.5 then ×2 on the card preview.
/// Remove after P1a is verified.
/// </summary>
public sealed class SulkTestCard() : DundunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<SulkingPower>(ctx, Owner.Creature, 3m, Owner.Creature, this);
}
