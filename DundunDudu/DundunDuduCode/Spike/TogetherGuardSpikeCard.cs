using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

// TEMP spike card: suppress BaseLib analyzer gates that don't matter for a throwaway co-op proof.
//   STS001 = localization entry required (real cards get proper loc in Phase 3/6; see localization/spike.json).
//   STS004 = card not added to a pool (intentional — added via dev console `card` for the test).
#pragma warning disable STS001, STS004

namespace DundunDudu.DundunDuduCode.Spike;

/// <summary>
/// Phase 2 Route-A co-op spike card (TEMPORARY — remove/replace when real 双人连携 content lands in Phase 5).
///
/// Goal: prove the cross-player mechanic compiles + works MP-safe. 墩墩 plays this; the SELECTED ally
/// (嘟嘟) gains Block — a measurable effect applied to the OTHER player's creature.
///
/// Why this is MP-safe (Route A, no custom net messages):
/// - <see cref="TargetType.AnyAlly"/> is a first-class game target ("any player excluding itself";
///   v0.107.1 TargetType.cs). BaseLib's AnyPlayerCardTargetingHelper + Harmony patches on
///   CardModel.IsValidTarget / NCardPlay.TryPlayCard make it selectable on the partner in multiplayer.
/// - The card play runs through the game's synchronized action queue (deterministic lockstep), and Block
///   is core combat state synced by CombatStateSynchronizer — so the effect lands identically on both
///   clients. No System.Random / no wall-clock / no direct remote-state poke.
///
/// Live verification (two instances + partner) is a manual step — see tools/coop-spike-test.md.
/// </summary>
// Colorless pool = available to any character + registered for the dev console `card` command,
// so the spike can be added in a co-op combat regardless of the characters picked.
[Pool(typeof(ColorlessCardPool))]
public sealed class TogetherGuardSpikeCard : ConstructedCardModel
{
    public TogetherGuardSpikeCard()
        : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyAlly)
    {
        WithBlock(8);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // AnyAlly => cardPlay.Target is the selected partner's creature.
        var ally = cardPlay.Target;
        if (ally == null)
        {
            // Solo / no second player: no valid ally — no-op gracefully.
            return;
        }

        await CreatureCmd.GainBlock(ally, DynamicVars.Block, cardPlay);
    }
}
