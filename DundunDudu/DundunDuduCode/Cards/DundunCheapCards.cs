using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 墩墩 salvaged generic cards (redesign P-prep). Originally 18; the Thorns engine (反手回击/见招拆招/倒刺护甲)
// and 3 off-identity cards (铜墙铁壁/心如止水/文火慢炖) were cut for the 情绪天平 redesign — see Dundun SPEC §4.
// These 12 are deterministic + single-player safe; they get reskinned into 猪饲料/麦霸/兴趣 (+闷气/tag clauses)
// in P3. Cross-player logic is Phase 5. Grouped one-file-many-classes; [Pool] from the DundunCard base.

/// 抓握点 — Skill 0, 3 block.
public sealed class Grip() : DundunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
}

/// 滋补汤 — Skill 1, 4 block + 2 Regen.
public sealed class NourishingSoup() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move), new PowerVar<RegenPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<RegenPower>(ctx, Owner.Creature, 2m, Owner.Creature, this);
    }
}

/// 稳扎一拳 — Attack 1, 7 damage.
public sealed class SteadyPunch() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

/// 缠斗 — Skill 1, 2 Weak to an enemy.
public sealed class Grapple() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(ctx, cardPlay.Target, 2m, Owner.Creature, this);
    }
}

/// 储备 — Skill 1, 5 block + draw 1.
public sealed class Reserve() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// 养生 — Skill 1, heal 4 + 1 Regen.
public sealed class Wellness() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RegenPower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 4m);
        await PowerCmd.Apply<RegenPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
    }
}

/// 防守垫步 — Skill 1, 6 block; 9 if you already have Block (Cheap self-state condition; reskinned in P3).
public sealed class DefensiveStep() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        decimal block = Owner.Creature.Block > 0 ? 9m : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }
}

/// 对冲 — Skill 2, 10 block + heal 3.
public sealed class Hedge() : DundunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CreatureCmd.Heal(Owner.Creature, 3m);
    }
}

/// 风控 — Skill 1, 1 Buffer.
public sealed class RiskControl() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>(1m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<BufferPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
}

/// 连续防守 — Skill 2, 2 Weak to all enemies + 6 block.
public sealed class SustainedDefense() : DundunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new PowerVar<WeakPower>(2m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        foreach (var enemy in base.CombatState!.HittableEnemies)
            await PowerCmd.Apply<WeakPower>(ctx, enemy, 2m, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}

/// 二段蹬腿 — Attack 2, 14 damage.
public sealed class DoubleKick() : DundunCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(14m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

/// 满汉全席 — Skill 3, 15 block + heal 8 + 4 Regen.
public sealed class GrandFeast() : DundunCard(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move), new PowerVar<RegenPower>(4m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CreatureCmd.Heal(Owner.Creature, 8m);
        await PowerCmd.Apply<RegenPower>(ctx, Owner.Creature, 4m, Owner.Creature, this);
    }
}
