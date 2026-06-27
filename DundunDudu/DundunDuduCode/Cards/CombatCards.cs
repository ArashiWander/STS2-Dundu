using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 极限格斗流 (H) — archetype resource 熟悉 (FamiliarPower). Deterministic, single-player safe. [Pool] from DundunCard.

/// 以伤换伤 — Attack 1, 6 dmg to an enemy + 2 unblockable self-damage (feeds 熟悉 via the unblocked hit).
public sealed class BloodForBlood() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        await CreatureCmd.Damage(ctx, Owner.Creature, 2m, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
    }
}

/// 千锤百炼 — Power 1, gain 千锤百炼 (doubles 熟悉's attack bonus to +2/stack for the combat).
public sealed class TemperHammer() : DundunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<TemperedPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
}

/// 野兽之力 — Attack 2, 16 dmg.
public sealed class BeastForce() : DundunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}
