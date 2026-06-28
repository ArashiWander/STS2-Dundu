namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure relic trigger logic — NO game types, unit-tested. Keeps the per-turn counting decisions
/// (read from combat history by the relics) testable in isolation.</summary>
public static class RelicMath
{
    /// <summary>大鼠与美叽: draw when you've now played an even number (≥2) of same-cost cards this turn
    /// (so the 2nd, 4th, … same-cost card each draw 1).</summary>
    public static bool SameCostDraws(int sameCostCountThisTurn) => sameCostCountThisTurn >= 2 && sameCostCountThisTurn % 2 == 0;

    /// <summary>比比拉不: fires exactly once — on the play that first makes this turn contain BOTH ≥1 attack and
    /// ≥1 block-granting skill. Counts include the just-played card.</summary>
    public static bool ComboCompleted(bool currentIsAttack, bool currentIsBlockSkill, int attacksThisTurn, int blockSkillsThisTurn)
        => (currentIsAttack && attacksThisTurn == 1 && blockSkillsThisTurn >= 1)
        || (currentIsBlockSkill && blockSkillsThisTurn == 1 && attacksThisTurn >= 1);

    /// <summary>小白菜: its extra 闷气 clear only triggers at 50% max HP or below.</summary>
    public static bool HalfHpOrLess(int currentHp, int maxHp) => maxHp > 0 && currentHp * 2 <= maxHp;

    /// <summary>香烟: restore up to 2 max HP per snack, never more than the remaining max-HP debt.</summary>
    public static int CigaretteRestore(int debt)
    {
        int d = debt < 0 ? 0 : debt;
        return d < 2 ? d : 2;
    }

    /// <summary>小熊虫: HP healed on the first lethal save — <paramref name="percent"/>% of max HP, at least 1.</summary>
    public static int LethalReviveHeal(int maxHp, int percent)
    {
        int m = maxHp < 0 ? 0 : maxHp;
        int h = m * percent / 100;
        return h < 1 ? 1 : h;
    }
}
