namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>
/// Pure 闷气 (Sulking) tier math — NO game/Godot types, so it is unit-tested standalone (tests/ links this exact
/// source file and asserts the boundaries). SulkingPower's hooks call these; keep all 闷气 numeric rules here.
///
/// Design (墩墩角色Mod设计文档1.md §2.1, 0–10):
///   0–2 平静  ×1.0,        no immunity
///   3–5 警觉  ×1.5 attack,  no immunity
///   6–8 爆发  ×2.0 attack,  immune to debuffs
///   9   临界  ×2.0,         immune; +1 穿透 self-damage at TURN START (warning)
///   10  彻底自闭 ×2.0,       NOT immune; (封技能 deferred); 3 穿透 self-damage at TURN END
/// </summary>
public static class SulkingMath
{
    public const int MaxStacks = 10;

    /// <summary>Clamp a raw stack count into the design's [0, 10] band.</summary>
    public static int Cap(int stacks) => stacks < 0 ? 0 : (stacks > MaxStacks ? MaxStacks : stacks);

    /// <summary>分层改伤: multiplier applied to 墩墩's OUTGOING attack damage. 3–5 → ×1.5, 6–10 → ×2.0, else ×1.0.</summary>
    public static decimal TierMultiplier(int stacks)
    {
        int s = Cap(stacks);
        return s >= 6 ? 2.0m : (s >= 3 ? 1.5m : 1.0m);
    }

    /// <summary>层数封顶: how much 闷气 a gain of <paramref name="requested"/> actually adds without passing 10 (never negative).</summary>
    public static int ClampGain(int current, int requested)
    {
        if (requested <= 0) return 0;
        int room = MaxStacks - Cap(current);
        if (room <= 0) return 0;
        return requested < room ? requested : room;
    }

    /// <summary>免疫负面: debuffs are negated while in 爆发/临界 (6–9). At 10 (彻底自闭) immunity drops — the punishment window.</summary>
    public static bool IsDebuffImmune(int stacks)
    {
        int s = Cap(stacks);
        return s >= 6 && s <= 9;
    }

    /// <summary>临界期(9): 穿透 self-damage taken at TURN START (1), else 0.</summary>
    public static int CriticalSelfDamage(int stacks) => Cap(stacks) == 9 ? 1 : 0;

    /// <summary>彻底自闭(10): 穿透 self-damage taken at TURN END (3), else 0.</summary>
    public static int MeltdownSelfDamage(int stacks) => Cap(stacks) == 10 ? 3 : 0;
}
