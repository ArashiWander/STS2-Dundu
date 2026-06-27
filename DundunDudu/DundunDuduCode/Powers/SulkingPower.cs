using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// 闷气 (Sulking) — 墩墩's core resource, 0–10 stacks. P1a scope: stacks + tiered OUTGOING-attack damage scaling.
/// Tiers: 0–2 平静 ×1.0 · 3–5 警觉 ×1.5 · 6–10 爆发 ×2.0. (免疫负面 6–8 / 封技能 10 / 穿透自伤 9·10 land in P1b.)
///
/// MP-safe by construction: a per-creature <see cref="PowerModel"/> whose stack count rides the synced combat
/// state; no static counters, no <c>System.Random</c>; 闷气 is personal, so it does NOT scale with player count
/// (<c>ShouldScaleInMultiplayer</c> left default false). Holds in 2-player co-op (墩墩 is one of two characters).
///
/// Pattern source: KnockdownPower (ModifyDamageMultiplicative direction), ArtifactPower (custom power shape).
/// </summary>
public sealed class SulkingPower : CustomPowerModel
{
    // Placeholder icons (P1a). BaseLib's CustomPowerModel uses these Custom*IconPath overrides (default null),
    // NOT ICustomPower.PackedIcon/BigIcon (those collide with PowerModel's Texture2D BigIcon). Real icon later.
    public override string? CustomPackedIconPath => "powers/power.png".ImagePath();
    public override string? CustomBigIconPath => "powers/big/power.png".ImagePath();

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Stack count clamped to the design's 0–10 range.</summary>
    private int Sulk => (int)System.Math.Min(base.Amount, 10m);

    /// <summary>
    /// 分层改伤: multiply ONLY the damage 墩墩 (owner) deals via a powered attack. 3–5 → ×1.5, 6–10 → ×2.0.
    /// Same hook + direction guard as KnockdownPower, but keyed on tier instead of a fixed multiplier.
    /// </summary>
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != base.Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }

        int s = Sulk;
        return s >= 6 ? 2.0m : (s >= 3 ? 1.5m : 1.0m);
    }
}
