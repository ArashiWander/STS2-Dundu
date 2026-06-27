namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure starter-card math — 闷气-scaled values for 大屁墩 / 墩坚强. NO game types, unit-tested.</summary>
public static class StarterMath
{
    /// <summary>大屁墩: 6 damage, +3 (→ 9) once 闷气 ≥ 3.</summary>
    public static int BigButtDamage(int sulkStacks) => sulkStacks >= 3 ? 9 : 6;

    /// <summary>墩坚强: 5 block + 2 per 3 闷气, bonus capped at +6 (so 5 / 7 / 9 / 11).</summary>
    public static int SteadfastBlock(int sulkStacks)
    {
        int s = sulkStacks < 0 ? 0 : sulkStacks;
        int extra = s / 3 * 2;
        if (extra > 6) extra = 6;
        return 5 + extra;
    }
}
