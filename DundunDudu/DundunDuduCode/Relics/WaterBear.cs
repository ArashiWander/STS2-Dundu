using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace DundunDudu.DundunDuduCode.Relics;

/// <summary>
/// 小熊虫 (Tardigrade) — 墩墩's Starter relic. The first time you would die this run, survive it and heal 10% max
/// HP (once). Clean lethal-damage interception via the SAME hooks the vanilla LizardTail uses: ShouldDieLate
/// returns false to cancel the death, AfterPreventingDeath heals and marks it spent. [SavedProperty] persists the
/// "used" flag across saves; IsUsedUp disables the relic afterward. MP note: per-creature; cross-player gated.
/// </summary>
public sealed class WaterBear : DundunRelic
{
    private bool _wasUsed;

    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool IsUsedUp => _wasUsed;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(10m)];

    [SavedProperty]
    public bool WasUsed
    {
        get => _wasUsed;
        set
        {
            AssertMutable();
            _wasUsed = value;
            if (IsUsedUp)
            {
                base.Status = RelicStatus.Disabled;
            }
        }
    }

    // Cancel the first lethal death (owner only, once). Mirrors LizardTail.ShouldDieLate.
    public override bool ShouldDieLate(Creature creature)
    {
        if (creature != base.Owner.Creature) return true;
        if (WasUsed) return true;
        return false;
    }

    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        WasUsed = true;
        int heal = RelicMath.LethalReviveHeal(creature.MaxHp, (int)base.DynamicVars.Heal.BaseValue);
        MainFile.Logger.Info($"[小熊虫] 首次免死 → 回复 {heal} (最大生命 {creature.MaxHp} 的 {(int)base.DynamicVars.Heal.BaseValue}%)");
        await CreatureCmd.Heal(creature, heal);
    }
}
