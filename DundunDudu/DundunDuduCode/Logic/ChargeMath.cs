namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 蓄力 (Charge) math — NO game types, unit-tested. 蓄力 is earned by ending a turn without
/// attacking and persists across turns; spenders read the stack count.</summary>
public static class ChargeMath
{
    /// <summary>斩断一切 consume-all payoff: 8 base + 4 per 蓄力 stack.</summary>
    public static int SlashAllDamage(int stacks) => 8 + 4 * (stacks < 0 ? 0 : stacks);
}
