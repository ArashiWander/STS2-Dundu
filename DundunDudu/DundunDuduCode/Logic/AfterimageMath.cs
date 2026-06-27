namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>
/// Pure 残影 (Afterimage) math — NO game types, unit-tested standalone. 残影 fully negates an incoming attack
/// hit (≥1) while stacks remain, consuming one stack per negated hit; unused stacks clear each turn (block rule).
/// </summary>
public static class AfterimageMath
{
    /// <summary>HP loss that actually applies: a hit of ≥1 is negated to 0 while stacks remain; otherwise unchanged.</summary>
    public static decimal HpLostAfter(int stacks, decimal incoming)
        => (stacks > 0 && incoming >= 1m) ? 0m : incoming;

    /// <summary>Whether this hit consumes exactly one 残影 stack (caller decrements once when true).</summary>
    public static bool Consumes(int stacks, decimal incoming) => stacks > 0 && incoming >= 1m;

    /// <summary>完美落地 payoff: consume-all → 6 per stack, with a 12 floor when you have no 残影.</summary>
    public static int PerfectLandingDamage(int stacks) => stacks > 0 ? 6 * stacks : 12;
}
