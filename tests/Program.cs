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

Console.WriteLine("== AfterimageMath.HpLostAfter (>=1 hit negated to 0 while stacks remain) ==");
Eq("HpLost(0,5)", AfterimageMath.HpLostAfter(0, 5m), 5m);
Eq("HpLost(1,5)", AfterimageMath.HpLostAfter(1, 5m), 0m);
Eq("HpLost(3,99)", AfterimageMath.HpLostAfter(3, 99m), 0m);
Eq("HpLost(1,1)", AfterimageMath.HpLostAfter(1, 1m), 0m);
Eq("HpLost(1,0.9)", AfterimageMath.HpLostAfter(1, 0.9m), 0.9m);
Eq("HpLost(2,0.5)", AfterimageMath.HpLostAfter(2, 0.5m), 0.5m);

Console.WriteLine("== AfterimageMath.Consumes (one stack per negated hit) ==");
Eq("Consumes(0,5)", AfterimageMath.Consumes(0, 5m), false);
Eq("Consumes(1,5)", AfterimageMath.Consumes(1, 5m), true);
Eq("Consumes(1,0.5)", AfterimageMath.Consumes(1, 0.5m), false);
Eq("Consumes(2,1)", AfterimageMath.Consumes(2, 1m), true);

Console.WriteLine("== AfterimageMath.PerfectLandingDamage (6/stack, 12 floor) ==");
Eq("Perfect(0)", AfterimageMath.PerfectLandingDamage(0), 12);
Eq("Perfect(1)", AfterimageMath.PerfectLandingDamage(1), 6);
Eq("Perfect(3)", AfterimageMath.PerfectLandingDamage(3), 18);
Eq("Perfect(5)", AfterimageMath.PerfectLandingDamage(5), 30);

Console.WriteLine("== ChargeMath.SlashAllDamage (8 + 4*stacks) ==");
Eq("Slash(0)", ChargeMath.SlashAllDamage(0), 8);
Eq("Slash(1)", ChargeMath.SlashAllDamage(1), 12);
Eq("Slash(3)", ChargeMath.SlashAllDamage(3), 20);
Eq("Slash(5)", ChargeMath.SlashAllDamage(5), 28);
Eq("Slash(-1→0)", ChargeMath.SlashAllDamage(-1), 8);

Console.WriteLine("== LastStandMath.IsDesperate (strictly < 30% max HP) ==");
Eq("Desperate(29,100)", LastStandMath.IsDesperate(29, 100), true);
Eq("Desperate(30,100)", LastStandMath.IsDesperate(30, 100), false);
Eq("Desperate(31,100)", LastStandMath.IsDesperate(31, 100), false);
Eq("Desperate(0,100)", LastStandMath.IsDesperate(0, 100), true);
Eq("Desperate(100,100)", LastStandMath.IsDesperate(100, 100), false);
Eq("Desperate(22,75)", LastStandMath.IsDesperate(22, 75), true);
Eq("Desperate(23,75)", LastStandMath.IsDesperate(23, 75), false);
Eq("Desperate(0,0)", LastStandMath.IsDesperate(0, 0), false);
Eq("Mult(29,100)", LastStandMath.DamageMultiplier(29, 100), 1.3m);
Eq("Mult(30,100)", LastStandMath.DamageMultiplier(30, 100), 1.0m);

Console.WriteLine("== FamiliarMath.Bonus (+1/stack, doubled when tempered) ==");
Eq("Bonus(0,false)", FamiliarMath.Bonus(0, false), 0);
Eq("Bonus(3,false)", FamiliarMath.Bonus(3, false), 3);
Eq("Bonus(3,true)", FamiliarMath.Bonus(3, true), 6);
Eq("Bonus(5,true)", FamiliarMath.Bonus(5, true), 10);
Eq("Bonus(0,true)", FamiliarMath.Bonus(0, true), 0);
Eq("Bonus(-1,false)", FamiliarMath.Bonus(-1, false), 0);

Console.WriteLine("== RelicMath.SameCostDraws (大鼠与美叽: even count >=2 draws) ==");
Eq("SameCost(0)", RelicMath.SameCostDraws(0), false);
Eq("SameCost(1)", RelicMath.SameCostDraws(1), false);
Eq("SameCost(2)", RelicMath.SameCostDraws(2), true);
Eq("SameCost(3)", RelicMath.SameCostDraws(3), false);
Eq("SameCost(4)", RelicMath.SameCostDraws(4), true);

Console.WriteLine("== RelicMath.ComboCompleted (比比拉不: fires once when attack+block pair completes) ==");
Eq("Combo(atk,a1,b1)", RelicMath.ComboCompleted(true, false, 1, 1), true);
Eq("Combo(atk,a1,b0)", RelicMath.ComboCompleted(true, false, 1, 0), false);
Eq("Combo(atk,a2,b1)", RelicMath.ComboCompleted(true, false, 2, 1), false);
Eq("Combo(blk,a1,b1)", RelicMath.ComboCompleted(false, true, 1, 1), true);
Eq("Combo(blk,a0,b1)", RelicMath.ComboCompleted(false, true, 0, 1), false);
Eq("Combo(blk,a1,b2)", RelicMath.ComboCompleted(false, true, 1, 2), false);

Console.WriteLine("== StarterMath.BigButtDamage (6, +3 at 闷气>=3) ==");
Eq("BigButt(0)", StarterMath.BigButtDamage(0), 6);
Eq("BigButt(2)", StarterMath.BigButtDamage(2), 6);
Eq("BigButt(3)", StarterMath.BigButtDamage(3), 9);
Eq("BigButt(10)", StarterMath.BigButtDamage(10), 9);
Eq("BigButt(-1)", StarterMath.BigButtDamage(-1), 6);

Console.WriteLine("== StarterMath.SteadfastBlock (5 + 2 per 3 闷气, cap +6) ==");
Eq("Steadfast(0)", StarterMath.SteadfastBlock(0), 5);
Eq("Steadfast(2)", StarterMath.SteadfastBlock(2), 5);
Eq("Steadfast(3)", StarterMath.SteadfastBlock(3), 7);
Eq("Steadfast(5)", StarterMath.SteadfastBlock(5), 7);
Eq("Steadfast(6)", StarterMath.SteadfastBlock(6), 9);
Eq("Steadfast(9)", StarterMath.SteadfastBlock(9), 11);
Eq("Steadfast(10)", StarterMath.SteadfastBlock(10), 11);
Eq("Steadfast(-1)", StarterMath.SteadfastBlock(-1), 5);

Console.WriteLine("== KaraokeMath.HighNoteDamage (16, or 22 if no skill played this turn) ==");
Eq("HighNote(playedSkill)", KaraokeMath.HighNoteDamage(true), 16);
Eq("HighNote(noSkill)", KaraokeMath.HighNoteDamage(false), 22);

Console.WriteLine("== SleepDeprivedMath.BlockReduction (min(stacks, block), floored 0) ==");
Eq("BlockRed(0,5)", SleepDeprivedMath.BlockReduction(0, 5), 0);
Eq("BlockRed(3,5)", SleepDeprivedMath.BlockReduction(3, 5), 3);
Eq("BlockRed(8,5)", SleepDeprivedMath.BlockReduction(8, 5), 5);
Eq("BlockRed(2,5)", SleepDeprivedMath.BlockReduction(2, 5), 2);
Eq("BlockRed(3,2)", SleepDeprivedMath.BlockReduction(3, 2), 2);
Eq("BlockRed(-1,5)", SleepDeprivedMath.BlockReduction(-1, 5), 0);
Eq("BlockRed(3,-1)", SleepDeprivedMath.BlockReduction(3, -1), 0);

Console.WriteLine("== SleepDeprivedMath.GivesSulking (>=3) ==");
Eq("GivesSulking(2)", SleepDeprivedMath.GivesSulking(2), false);
Eq("GivesSulking(3)", SleepDeprivedMath.GivesSulking(3), true);
Eq("GivesSulking(10)", SleepDeprivedMath.GivesSulking(10), true);
Eq("GivesSulking(0)", SleepDeprivedMath.GivesSulking(0), false);

Console.WriteLine($"\n{total - failures}/{total} passed, {failures} failed.");
return failures == 0 ? 0 : 1;
