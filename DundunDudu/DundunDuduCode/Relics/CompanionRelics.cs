using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Relics;

// 玩偶遗物 (P7, structural subset). Resource/tag relics (小粉/土豆/小白菜/香烟) need 闷气-events / SNACK tag /
// 熬大夜 (P2/P3, not built yet); 小熊虫 needs the Osty death system — both deferred. These 3 use confirmed hooks.
// NOTE: RelicModel.Owner is a Player; use base.Owner.Creature for Creature-typed APIs, base.Owner for Draw.

/// 卡皮吧啦 — Common. End of turn: gain Block equal to your leftover (unspent) energy.
public sealed class Capybara : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Contains(base.Owner.Creature)) return;
        int energy = base.Owner.PlayerCombatState!.Energy;
        if (energy <= 0) return;

        MainFile.Logger.Info($"[卡皮吧啦] 回合末剩余能量 {energy} → 格挡 +{energy}");
        await CreatureCmd.GainBlock(base.Owner.Creature, energy, ValueProp.Move, null);
    }
}

/// 大鼠与美叽 — Common. Each time you play your 2nd (4th, …) card of a given cost this turn, draw 1.
public sealed class MouseAndMagpie : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Creature me = base.Owner.Creature;
        int cost = cardPlay.Resources.EnergyValue;
        int sameCost = CombatHistory.CardsPlayedThisTurnWithCost(me, me.CombatState!, cost);
        if (!RelicMath.SameCostDraws(sameCost)) return;

        MainFile.Logger.Info($"[大鼠与美叽] 本回合第 {sameCost} 张 {cost} 费牌 → 抽 1");
        await CardPileCmd.Draw(choiceContext, 1, base.Owner);
    }
}

/// 比比拉不 — Uncommon. The turn you've played ≥1 attack AND ≥1 block-granting skill, gain 3 Block (once/turn).
public sealed class Bibilabu : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool isAttack = cardPlay.Card.Type == CardType.Attack;
        bool isBlockSkill = cardPlay.Card.Type == CardType.Skill && cardPlay.Card.GainsBlock;
        if (!isAttack && !isBlockSkill) return;

        Creature me = base.Owner.Creature;
        ICombatState cs = me.CombatState!;
        int attacks = CombatHistory.AttacksPlayedThisTurn(me, cs);
        int blockSkills = CombatHistory.BlockSkillsPlayedThisTurn(me, cs);
        if (!RelicMath.ComboCompleted(isAttack, isBlockSkill, attacks, blockSkills)) return;

        MainFile.Logger.Info($"[比比拉不] 刀盾连击 (攻{attacks}/盾{blockSkills}) → 格挡 +3");
        await CreatureCmd.GainBlock(me, 3m, ValueProp.Move, null);
    }
}
