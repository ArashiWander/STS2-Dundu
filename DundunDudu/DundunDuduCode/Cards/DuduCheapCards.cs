using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 嘟嘟 Cheap card pool (Phase 4 follow-on). Agile / combo / low-cost feel; existing powers/commands only.
// Deterministic + single-player safe — ZERO cross-player logic (synergy = Phase 5). Rough balance.
// NOTE: 越吃越快 (draw+energy each turn) and 停不下嘴 (on-4th-card trigger) are BUMPED to Medium — both need a
// custom power with turn/card hooks, no existing power fits — so they are NOT built here (per the spec rule).

/// 嘴馋 — Skill 0, draw 1.
public sealed class Peckish() : DuduCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await CardPileCmd.Draw(ctx, 1, Owner);
}

/// 戳一口 — Attack 0, 3 damage.
public sealed class Poke() : DuduCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

/// 两口吞 — Attack 1, 3 damage ×2.
public sealed class TwoBites() : DuduCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(2).Execute(ctx);
    }
}

/// 护食 — Skill 1, 5 block + 1 Dexterity.
public sealed class GuardFood() : DuduCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new PowerVar<DexterityPower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<DexterityPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
    }
}

/// 假装分你 — Attack 1, 4 damage + 1 Vulnerable.
public sealed class PretendShare() : DuduCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new PowerVar<VulnerablePower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        await PowerCmd.Apply<VulnerablePower>(ctx, cardPlay.Target, 1m, Owner.Creature, this);
    }
}

/// 加餐 — Skill 1, draw 2.
public sealed class ExtraServing() : DuduCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await CardPileCmd.Draw(ctx, 2, Owner);
}

/// 抱紧饭碗 — Skill 1, 7 block.
public sealed class HugBowl() : DuduCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
}

/// 开胃 — Power 1, 2 Dexterity.
public sealed class Appetizer() : DuduCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<DexterityPower>(ctx, Owner.Creature, 2m, Owner.Creature, this);
}

/// 边吃边看 — Skill 1, 6 block + draw 1.
public sealed class EatAndWatch() : DuduCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// 抢菜 — Skill 1, draw 2 + 1 energy.
public sealed class GrabDish() : DuduCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(ctx, 2, Owner);
        await PlayerCmd.GainEnergy(1m, Owner);
    }
}

/// 风卷残云 — Attack 2, 6 damage ×3.
public sealed class Whirlwind() : DuduCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(3).Execute(ctx);
    }
}

/// 大快朵颐 — Attack 2, 4 damage ×2 + draw 1.
public sealed class Feast() : DuduCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(2).Execute(ctx);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// 横扫自助 — Attack 2, 5 damage to all enemies + 1 Vulnerable each.
public sealed class SweepBuffet() : DuduCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move), new PowerVar<VulnerablePower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState!).Execute(ctx);
        foreach (var enemy in base.CombatState!.HittableEnemies)
            await PowerCmd.Apply<VulnerablePower>(ctx, enemy, 1m, Owner.Creature, this);
    }
}

/// 边吃边抢 — Attack 1, 6 damage; if you have Dexterity, gain 1 energy. (Spec said "若本回合≥3张牌" but
/// there is no Cheap ambient "cards-this-turn" counter; reinterpreted to 嘟嘟's own Dexterity-combo state —
/// Cheap, on-theme, no power.)
public sealed class EatAndGrab() : DuduCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        if (Owner.Creature.GetPower<DexterityPower>() != null)
            await PlayerCmd.GainEnergy(1m, Owner);
    }
}

/// 大餐 — Skill 2, heal 10.
public sealed class BigMeal() : DuduCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await CreatureCmd.Heal(Owner.Creature, 10m);
}

/// 吃饱不怕 — Skill 2, Intangible 1 turn.
public sealed class Fearless() : DuduCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<IntangiblePower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
}

/// 报复性进食 — Attack 3, 4 damage ×5.
public sealed class RevengeEating() : DuduCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitCount(5).Execute(ctx);
    }
}
