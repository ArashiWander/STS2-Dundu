using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Cards;

// 兴趣与穿搭 (C) — the special cards (mechanical / card-generation / cleanse). Deterministic apart from 机械键盘's
// synced-random pulse. Cross-player untouched. [Pool] from DundunCard.

/// 机械键盘 — Power 2, gain 机械键盘 (every 4 cards → 8 to a random enemy).
public sealed class MechKeyboard() : DundunCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
        => await PowerCmd.Apply<MechKeyboardPower>(ctx, Owner.Creature, 1m, Owner.Creature, this);
}

/// KICK BACK — Attack 2, 18 damage; shuffle a 0-cost 大屁墩！ into your draw pile.
public sealed class KickBack() : DundunCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);

        CardModel bigButt = Owner.RunState.CreateCard(ModelDb.Card<BigButt>(), Owner);
        bigButt.SetStarCostThisCombat(0);
        await CardPileCmd.Add(bigButt, PileType.Draw, CardPilePosition.Random);
    }
}

/// 联机游戏 — Attack 1, 8 damage + 1 energy.
public sealed class OnlineGaming() : DundunCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(ctx);
        await PlayerCmd.GainEnergy(1m, Owner);
    }
}

/// 芝士味Dorito — Skill 1 (Snack), 2 energy + shuffle a junk card (vanilla Slimed, our 黏糊糊) into your draw pile.
public sealed class CheesyDorito() : DundunCard(1, CardType.Skill, CardRarity.Common, TargetType.Self), ITaggedDundunCard
{
    public IReadOnlySet<DundunCardTag> DundunTags => DundunCardTags.SnackSet;
    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(2m, Owner);
        CardModel junk = Owner.RunState.CreateCard(ModelDb.Card<Slimed>(), Owner);
        await CardPileCmd.Add(junk, PileType.Draw, CardPilePosition.Random);
    }
}

/// 喷洒：银色山泉 — Skill 1, Exhaust. Remove all your debuffs; halve 闷气 into that many 人工制品; you can't play
/// attacks this turn.
public sealed class SilverSpring() : DundunCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        Creature me = Owner.Creature;

        // 清除全部负面状态（含【熬大夜】）
        foreach (PowerModel p in me.Powers.Where(p => p.Type == PowerType.Debuff).ToList())
        {
            await PowerCmd.Remove(p);
        }

        // 闷气减半，转化为等量人工制品
        int artifact = SilverSpringMath.ArtifactFromSulking((int)(me.GetPower<SulkingPower>()?.Amount ?? 0m));
        if (artifact > 0)
        {
            await Sulking.Clear(ctx, me, artifact);
            await PowerCmd.Apply<ArtifactPower>(ctx, me, artifact, me, this);
        }

        // 代价：本回合不能打出攻击卡
        await PowerCmd.Apply<NoAttacksPower>(ctx, me, 1m, me, this);
    }
}
