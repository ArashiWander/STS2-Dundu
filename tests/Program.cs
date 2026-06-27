using DundunDudu.DundunDuduCode.Logic;

// Self-contained assert runner for the mod's pure formula logic. Prints one line per case and exits non-zero
// on any failure, so smoke/CI and `dotnet run` both surface regressions. Add a group per custom power as built.

int failures = 0, total = 0;

void Eq(string name, object actual, object expected)
{
    total++;
    bool ok = Equals(actual, expected);
    if (!ok) failures++;
    Console.WriteLine($"{(ok ? "  ok " : "FAIL")} {name}  =>  got {actual}, want {expected}");
}

Console.WriteLine("== SulkingMath.Cap (clamp to [0,10]) ==");
Eq("Cap(-5)", SulkingMath.Cap(-5), 0);
Eq("Cap(-1)", SulkingMath.Cap(-1), 0);
Eq("Cap(0)", SulkingMath.Cap(0), 0);
Eq("Cap(5)", SulkingMath.Cap(5), 5);
Eq("Cap(10)", SulkingMath.Cap(10), 10);
Eq("Cap(11)", SulkingMath.Cap(11), 10);
Eq("Cap(100)", SulkingMath.Cap(100), 10);

Console.WriteLine("== SulkingMath.TierMultiplier (0-2 x1.0 / 3-5 x1.5 / 6-10 x2.0) ==");
Eq("Tier(0)", SulkingMath.TierMultiplier(0), 1.0m);
Eq("Tier(2)", SulkingMath.TierMultiplier(2), 1.0m);
Eq("Tier(3)", SulkingMath.TierMultiplier(3), 1.5m);
Eq("Tier(5)", SulkingMath.TierMultiplier(5), 1.5m);
Eq("Tier(6)", SulkingMath.TierMultiplier(6), 2.0m);
Eq("Tier(8)", SulkingMath.TierMultiplier(8), 2.0m);
Eq("Tier(9)", SulkingMath.TierMultiplier(9), 2.0m);
Eq("Tier(10)", SulkingMath.TierMultiplier(10), 2.0m);
Eq("Tier(11→cap)", SulkingMath.TierMultiplier(11), 2.0m);
Eq("Tier(-1→cap)", SulkingMath.TierMultiplier(-1), 1.0m);

Console.WriteLine("== SulkingMath.ClampGain (never past 10, never negative) ==");
Eq("ClampGain(0,3)", SulkingMath.ClampGain(0, 3), 3);
Eq("ClampGain(8,3)", SulkingMath.ClampGain(8, 3), 2);
Eq("ClampGain(10,3)", SulkingMath.ClampGain(10, 3), 0);
Eq("ClampGain(9,5)", SulkingMath.ClampGain(9, 5), 1);
Eq("ClampGain(3,7)", SulkingMath.ClampGain(3, 7), 7);
Eq("ClampGain(7,3)", SulkingMath.ClampGain(7, 3), 3);
Eq("ClampGain(0,0)", SulkingMath.ClampGain(0, 0), 0);
Eq("ClampGain(5,-2)", SulkingMath.ClampGain(5, -2), 0);
Eq("ClampGain(10,1)", SulkingMath.ClampGain(10, 1), 0);

Console.WriteLine("== SulkingMath.IsDebuffImmune (6-9 only) ==");
Eq("Immune(5)", SulkingMath.IsDebuffImmune(5), false);
Eq("Immune(6)", SulkingMath.IsDebuffImmune(6), true);
Eq("Immune(8)", SulkingMath.IsDebuffImmune(8), true);
Eq("Immune(9)", SulkingMath.IsDebuffImmune(9), true);
Eq("Immune(10)", SulkingMath.IsDebuffImmune(10), false);
Eq("Immune(0)", SulkingMath.IsDebuffImmune(0), false);

Console.WriteLine("== SulkingMath.CriticalSelfDamage (turn START, 9→1) ==");
Eq("Crit(8)", SulkingMath.CriticalSelfDamage(8), 0);
Eq("Crit(9)", SulkingMath.CriticalSelfDamage(9), 1);
Eq("Crit(10)", SulkingMath.CriticalSelfDamage(10), 0);

Console.WriteLine("== SulkingMath.MeltdownSelfDamage (turn END, 10→3) ==");
Eq("Meltdown(9)", SulkingMath.MeltdownSelfDamage(9), 0);
Eq("Meltdown(10)", SulkingMath.MeltdownSelfDamage(10), 3);
Eq("Meltdown(11→cap)", SulkingMath.MeltdownSelfDamage(11), 3);

Console.WriteLine($"\n{total - failures}/{total} passed, {failures} failed.");
return failures == 0 ? 0 : 1;
