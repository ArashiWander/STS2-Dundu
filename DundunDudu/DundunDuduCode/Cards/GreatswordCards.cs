using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 巨剑黑暗流 (G) — archetype resource 蓄力 (ChargePower). Deterministic, single-player safe. [Pool] from DundunCard.

/// 沉重步伐 — Skill 0, 3 block + draw 1 (cheap no-attack cycle that helps bank 蓄力).
public sealed class HeavyStep() : DundunCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override bool GainsBlock => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(3m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.Draw(ctx, 1, Owner);
    }
}

/// 斩断一切 — Attack 2, consume ALL 蓄力, deal 8 + 4×stacks. Preview shows the 8 base; actual scales by 蓄力.
public sealed class CleaveAll() : DundunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        int stacks = (int)(Owner.Creature.GetPower<ChargePower>()?.Amount ?? 0m);
        int dmg = ChargeMath.SlashAllDamage(stacks);
        if (stacks > 0) await PowerCmd.Remove<ChargePower>(Owner.Creature);
        await DamageCmd.Attack(dmg).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
    }
}

/// 绝境坚持 — Power 1, gain LastStand (while HP &lt; 30%, attack damage +30%).
public sealed class LastStand() : DundunCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<LastStandPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
}
