using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// 墩坚强 — 墩墩's basic Defend (reskinned). 5 block + 2 per 3 闷气, bonus capped at +6 (so 5 / 7 / 9 / 11).
/// Defend-tagged + Basic. Block value in unit-tested <see cref="StarterMath"/>. Preview shows the 5 base; the
/// 闷气 bonus applies on play.
/// </summary>
public sealed class Steadfast() : DundunCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        int sulk = (int)(Owner.Creature.GetPower<SulkingPower>()?.Amount ?? 0m);
        await CreatureCmd.GainBlock(Owner.Creature, StarterMath.SteadfastBlock(sulk), ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
