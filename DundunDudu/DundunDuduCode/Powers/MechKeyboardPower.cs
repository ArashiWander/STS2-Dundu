using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 机械键盘 (Mechanical Keyboard) — the 6th custom power. Every 4 cards the owner plays, a "清脆敲击" deals 8
/// damage to a random enemy (synced random via DamageCmd's TargetingRandomOpponents — MP-safe RNG). The counter
/// rides per-instance internal data (FeralPower pattern), synced with the power → MP-safe. Single stack-type
/// (passive presence; the 0–3 progress isn't shown).
///
/// Deviation: the design says 穿透 (Unblockable), but the attack builder exposes no Unblockable, so the pulse is
/// ordinary (Unpowered, flat 8) synced-random damage. Flagged in VERIFY-QUEUE.
/// </summary>
public sealed class MechKeyboardPower : CustomPowerModel
{
    private sealed class Data { public int CardsSincePulse; }

    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override object InitInternalData() => new Data();

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner) return;

        Data data = GetInternalData<Data>();
        data.CardsSincePulse++;
        if (data.CardsSincePulse < 4) return;

        data.CardsSincePulse = 0;
        MainFile.Logger.Info("[机械键盘] 清脆敲击 → 随机敌人 8 伤");
        await DamageCmd.Attack(8m).FromCard(cardPlay.Card).Unpowered()
            .TargetingRandomOpponents(base.Owner.CombatState!).Execute(choiceContext);
    }
}
