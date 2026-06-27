namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 绝境坚持 (Last Stand) math — NO game types, unit-tested. "生命低于30%" = strictly below 30%
/// of max HP; while active, attack damage is ×1.3.</summary>
public static class LastStandMath
{
    public static bool IsDesperate(int currentHp, int maxHp) => maxHp > 0 && currentHp * 100 < maxHp * 30;

    public static decimal DamageMultiplier(int currentHp, int maxHp) => IsDesperate(currentHp, maxHp) ? 1.3m : 1.0m;
}
