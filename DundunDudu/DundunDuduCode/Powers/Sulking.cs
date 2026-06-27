using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// The single entry point for GAINING and CLEARING 闷气. Gains clamp to ≤10 via the unit-tested
/// <see cref="SulkingMath.ClampGain"/>; clears decrement per-stack. Stateless static methods — no counters,
/// no RNG — so MP-safe. Every 墩墩 card that touches 闷气 goes through here.
/// </summary>
public static class Sulking
{
    public static async Task Gain(PlayerChoiceContext ctx, Creature owner, int requested, CardModel? source)
    {
        int current = (int)(owner.GetPower<SulkingPower>()?.Amount ?? 0m);
        int delta = SulkingMath.ClampGain(current, requested);
        MainFile.Logger.Info($"[闷气] gain req={requested} cur={current} → +{delta}");
        if (delta <= 0) return;
        await PowerCmd.Apply<SulkingPower>(ctx, owner, delta, owner, source);
    }

    /// <summary>Clear up to <paramref name="n"/> 闷气 (capped at the current amount). MP-safe: synced Decrements.</summary>
    public static async Task Clear(PlayerChoiceContext ctx, Creature owner, int n)
    {
        var power = owner.GetPower<SulkingPower>();
        if (power == null || n <= 0) return;
        int times = System.Math.Min(n, (int)power.Amount);
        if (times <= 0) return;

        MainFile.Logger.Info($"[闷气] 清空 {times} 层 (从 {(int)power.Amount})");
        for (int i = 0; i < times; i++)
        {
            await PowerCmd.Decrement(power);
        }
    }
}
