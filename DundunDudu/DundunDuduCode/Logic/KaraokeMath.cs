namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 麦霸 card math — NO game types, unit-tested.</summary>
public static class KaraokeMath
{
    /// <summary>高音炫技: 16 damage, boosted to 22 when you've played NO skill this turn.</summary>
    public static int HighNoteDamage(bool playedSkillThisTurn) => playedSkillThisTurn ? 16 : 22;
}
