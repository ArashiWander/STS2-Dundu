using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// P1a/P1b TEST card (TEMP): add 3 闷气 to 墩墩 (via the clamping <see cref="Sulking.Gain"/> helper). Play once →
/// 警觉(3–5, ×1.5); twice → 爆发(6, ×2.0); four times → caps at 10 (彻底自闭, 封技能 deferred, 回合末 3 穿透).
/// Remove after the 闷气 tiers are verified in-game.
/// </summary>
public sealed class SulkTestCard() : DundunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await Sulking.Gain(ctx, Owner.Creature, 3, this);
}
