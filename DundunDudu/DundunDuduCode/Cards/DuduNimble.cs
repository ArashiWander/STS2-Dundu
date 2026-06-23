using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// 嘟嘟 signature setup — 0-cost, gain 1 Dexterity on yourself. Cheap "setup/combo" enabler that makes the
/// low-block agile deck scale. Deterministic, single-player safe (Self target only).
/// </summary>
public sealed class DuduNimble() : DuduCard(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Fixed +1 Dexterity to self (amount matches the PowerVar used for the {DexterityPower} description token).
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
