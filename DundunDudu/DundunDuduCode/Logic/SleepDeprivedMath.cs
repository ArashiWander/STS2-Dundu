namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 熬大夜 (Sleep-Deprived) math — NO game types, unit-tested.</summary>
public static class SleepDeprivedMath
{
    /// <summary>熬大夜: the turn's FIRST block card loses block equal to the stack count, floored so block ≥ 0.</summary>
    public static int BlockReduction(int stacks, int block)
    {
        int s = stacks < 0 ? 0 : stacks;
        int b = block < 0 ? 0 : block;
        return s < b ? s : b;
    }

    /// <summary>闷气联动: ≥3 stacks → gain +1 闷气 at turn start.</summary>
    public static bool GivesSulking(int stacks) => stacks >= 3;
}
