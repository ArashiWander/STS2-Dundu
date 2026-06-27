using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// 大屁墩！ — 墩墩's basic Strike (reskinned from the placeholder). 6 damage; +3 (→ 9) once 闷气 ≥ 3. Strike-tagged
/// + Basic rarity so Strike-referencing content (LargeCapsule etc.) resolves. Damage value in unit-tested
/// <see cref="StarterMath"/>. Preview shows the 6 base; the +3 applies on play (闷气 read at resolution).
/// </summary>
public sealed class BigButt() : DundunCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int sulk = (int)(Owner.Creature.GetPower<SulkingPower>()?.Amount ?? 0m);
        await DamageCmd.Attack(StarterMath.BigButtDamage(sulk)).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
