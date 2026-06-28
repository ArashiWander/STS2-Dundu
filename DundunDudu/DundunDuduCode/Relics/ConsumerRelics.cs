using DundunDudu.DundunDuduCode.Cards;
using DundunDudu.DundunDuduCode.Logic;
using DundunDudu.DundunDuduCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

namespace DundunDudu.DundunDuduCode.Relics;

// Tag/event-consumer relics — build on the now-built tag system + 闷气. Single-player safe; cross-player gated.

/// 小白菜 — Uncommon. Playing a 猪饲料 (Snack) card heals +3; at ≤50% max HP it also clears 2 闷气.
public sealed class BabyBokChoy : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!DundunCardTags.IsSnack(cardPlay.Card)) return;

        Creature me = base.Owner.Creature;
        MainFile.Logger.Info("[小白菜] 猪饲料 → +3 生命");
        await CreatureCmd.Heal(me, 3m);
        if (RelicMath.HalfHpOrLess(me.CurrentHp, me.MaxHp))
        {
            MainFile.Logger.Info("[小白菜] 生命≤50% → 清空 2 闷气");
            await Sulking.Clear(choiceContext, me, 2);
        }
    }
}

/// 小粉 — Uncommon. Marker relic; its "heal 2 each time 闷气 is cleared" lives in <see cref="Sulking.Clear"/>
/// (the single chokepoint through which every 闷气 clear flows).
public sealed class XiaoFen : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
}

/// 被嘟嘟没收的香烟 — Event relic. It IS registered in DundunRelicPool (so the analyzer is happy), but Event
/// rarity means RollRarity (Common/Uncommon/Rare) never offers it as a normal combat reward — it's event/console
/// only, matching "事件获取不进常规池". Reworked to a max-HP debt instead of 熬大夜 (so 闷气's 免疫负面 can never
/// dodge the downside): each combat start lose 6 max HP; each 猪饲料 card repays up to 2; any leftover debt is
/// repaid at combat end so it never spirals down.
public sealed class ConfiscatedCigarette : DundunRelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    private int _maxHpDebt;

    public override async Task BeforeCombatStart()
    {
        _maxHpDebt = 6;
        MainFile.Logger.Info($"[香烟] 开战戒断：最大生命 -{_maxHpDebt}");
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), base.Owner.Creature, _maxHpDebt, false);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_maxHpDebt <= 0 || !DundunCardTags.IsSnack(cardPlay.Card)) return;

        int restore = RelicMath.CigaretteRestore(_maxHpDebt);
        _maxHpDebt -= restore;
        MainFile.Logger.Info($"[香烟] 猪饲料回上限 +{restore} (剩余债 {_maxHpDebt})");
        await CreatureCmd.GainMaxHp(base.Owner.Creature, restore);
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (_maxHpDebt <= 0) return;

        int restore = _maxHpDebt;
        _maxHpDebt = 0;
        MainFile.Logger.Info($"[香烟] 战斗结束还清上限债 +{restore}");
        await CreatureCmd.GainMaxHp(base.Owner.Creature, restore);
    }
}
