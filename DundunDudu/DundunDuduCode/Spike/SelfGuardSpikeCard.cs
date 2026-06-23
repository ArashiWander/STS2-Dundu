using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DundunDudu.DundunDuduCode.Spike;

/// <summary>
/// Solo card-logic sibling of <see cref="TogetherGuardSpikeCard"/> (TEMPORARY).
/// <c>TargetType.Self</c> → gain Block on YOURSELF.
///
/// Purpose: verify the block-granting card LOGIC works in **single-player**, isolating it from
/// multiplayer. If this card works solo but TogetherGuard fails in co-op, the failure is *sync*,
/// not card logic (see tools/coop-spike-test.md §3 "local-before-remote" diagnostic).
///
/// Note: `WithBlock(8)` only declares the Block DynamicVar (for the `{Block}` description + block
/// frame); it does NOT auto-apply — the explicit <c>OnPlay</c> is what grants the Block.
/// </summary>
[Pool(typeof(ColorlessCardPool))]
public sealed class SelfGuardSpikeCard : ConstructedCardModel
{
    public SelfGuardSpikeCard()
        : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(8);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Self => apply to the owner's own creature.
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}
