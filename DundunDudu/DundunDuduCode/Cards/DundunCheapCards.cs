using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 墩墩 P3b — the 12 salvaged cards reskinned onto the new identity: 猪饲料 (Snack) / 麦霸 (Karaoke) / 兴趣 (no tag),
// now carrying 闷气 gain/clear and the mod-internal Snack/Karaoke tags. Deterministic + single-player safe;
// cross-player (情侣/D) untouched. 闷气 always flows through the Sulking helper (gain clamps to 10, clear decrements).

// ── 麦霸系列 (Karaoke) ───────────────────────────────────────────────────────────────────────────────────────

/// 关起门来吼两嗓子 — Skill 1, 清空2闷气 + 3 block.
public sealed class ShoutItOut() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.KaraokeSet;
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Sulking.Clear(ctx, Owner.Creature, 2);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }
}

/// 五音不全也要唱 — Attack 1, 7 dmg; 闷气≥3 时额外清空 1 闷气.
public sealed class OffKeySong() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.KaraokeSet;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        if ((int)(Owner.Creature.GetPower<SulkingPower>()?.Amount ?? 0m) >= 3)
            await Sulking.Clear(ctx, Owner.Creature, 1);
    }
}

/// 一个人的卡拉OK — Skill 1, heal 5 + 清空3闷气.
public sealed class SoloKaraoke() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.KaraokeSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 5m);
        await Sulking.Clear(ctx, Owner.Creature, 3);
    }
}

/// 副歌大合唱 — Attack 1, 6 dmg to target + 3 to all enemies.
public sealed class ChorusAllSing() : DundunCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.KaraokeSet;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        foreach (var enemy in base.CombatState!.HittableEnemies)
            await DamageCmd.Attack(3m).FromCard(this).Targeting(enemy).Execute(ctx);
    }
}

/// 高音炫技 — Attack 2, 16 dmg; 本回合未打技能牌 → 22 dmg.
public sealed class HighNote() : DundunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.KaraokeSet;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        bool playedSkill = CombatHistory.SkillsPlayedThisTurn(Owner.Creature, base.CombatState!) > 0;
        await DamageCmd.Attack(KaraokeMath.HighNoteDamage(playedSkill)).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

// ── 猪饲料系列 (Snack) ───────────────────────────────────────────────────────────────────────────────────────

/// 正宗上海本帮糖醋排骨 — Skill 1, heal 6 + 清空2闷气.
public sealed class SweetSourRibs() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 6m);
        await Sulking.Clear(ctx, Owner.Creature, 2);
    }
}

/// 牛排（五分熟）— Skill 1, next attack +4 damage (VigorPower, reflavored from the design's "+50%") + 1 闷气.
public sealed class Steak() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>(4m)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(ctx, Owner.Creature, 4m, Owner.Creature, this);
        await Sulking.Gain(ctx, Owner.Creature, 1, this);
    }
}

/// 蛋饼 — Skill 0, 清空1闷气 + draw 1.
public sealed class EggPancake() : DundunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await Sulking.Clear(ctx, Owner.Creature, 1);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// Pizza — Skill 1, heal 7 + 2 energy.
public sealed class Pizza() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 7m);
        await PlayerCmd.GainEnergy(2m, Owner);
    }
}

/// 巴西酸奶油牛肉（Strogonoff）— Skill 2, heal 12 + 1 闷气. (土豆/迟钝 clauses deferred with their relic.)
public sealed class Strogonoff() : DundunCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, 12m);
        await Sulking.Gain(ctx, Owner.Creature, 1, this);
    }
}

// ── 兴趣系列 (no tag) ────────────────────────────────────────────────────────────────────────────────────────

/// 限定版手办上架 — Skill 1, 5 block; if you already have Block, +3 (Cheap proxy for the design's "未受伤" clause).
public sealed class FigureDrop() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        decimal block = Owner.Creature.Block > 0 ? 8m : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }
}

/// 长款皮衣 — Skill 2, 10 block + heal 3.
public sealed class LongCoat() : DundunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(10m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CreatureCmd.Heal(Owner.Creature, 3m);
    }
}
