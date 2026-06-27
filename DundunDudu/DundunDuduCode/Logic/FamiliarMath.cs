namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 熟悉 (Familiar) math — NO game types, unit-tested. 熟悉 grants +1 attack damage per stack,
/// doubled to +2 per stack while 千锤百炼 (tempered) is active.</summary>
public static class FamiliarMath
{
    public static int Bonus(int stacks, bool tempered) => (stacks < 0 ? 0 : stacks) * (tempered ? 2 : 1);
}
