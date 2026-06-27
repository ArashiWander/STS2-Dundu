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
}
