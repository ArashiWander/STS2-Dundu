using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 太空步系列 (F) — archetype resource 残影 (AfterimageStacksPower). Deterministic, single-player safe.
// [Pool] inherited from DundunCard. 残影 clears each turn, so "you have 残影" == "gained 残影 this turn".

/// 凌空倒退 — Skill 1, gain 2 残影.
public sealed class AerialRetreat() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<AfterimageStacksPower>(ctx, Owner.Creature, 2m, Owner.Creature, this);
}

/// 太空步连切 — Attack 1, 9 dmg; if you already have 残影 (== gained it this turn), gain +1 残影.
public sealed class MoonwalkSlash() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        if ((Owner.Creature.GetPower<AfterimageStacksPower>()?.Amount ?? 0m) > 0m)
            await PowerCmd.Apply<AfterimageStacksPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
    }
}

/// 镜花水月 — Skill 2, gain 3 残影 + draw 1.
public sealed class MirrorImage() : DundunCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AfterimageStacksPower>(ctx, Owner.Creature, 3m, Owner.Creature, this);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// 舞台聚光灯 — Skill 1, 6 block; if you've played an attack this turn, 9 block instead (FeralPower history read).
public sealed class Spotlight() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        bool playedAttack = CombatHistory.AttacksPlayedThisTurn(Owner.Creature, base.CombatState!) > 0;
        decimal block = playedAttack ? 9m : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }
}

/// 完美落地 — Attack 2, consume ALL 残影, deal 6 per stack (12 floor if none). Preview shows the 12 floor.
public sealed class PerfectLanding() : DundunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int stacks = (int)(Owner.Creature.GetPower<AfterimageStacksPower>()?.Amount ?? 0m);
        int dmg = AfterimageMath.PerfectLandingDamage(stacks);
        if (stacks > 0) await PowerCmd.Remove<AfterimageStacksPower>(Owner.Creature);
        await DamageCmd.Attack(dmg).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

/// 华丽转身 — Attack 1, 7 dmg then gain 1 残影.
public sealed class GracefulTurn() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        await PowerCmd.Apply<AfterimageStacksPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
    }
}
