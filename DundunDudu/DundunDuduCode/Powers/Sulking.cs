using DundunDudu.DundunDuduCode.Logic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DundunDudu.DundunDuduCode.Powers;

/// <summary>
/// The single entry point for GAINING 闷气. Clamps the requested amount via the unit-tested
/// <see cref="SulkingMath.ClampGain"/> so stacks never exceed 10 (层数封顶 at the source). Stateless static
/// method — no counters, no RNG — so it stays MP-safe. Every 墩墩 card that grants 闷气 calls this rather than
/// <c>PowerCmd.Apply&lt;SulkingPower&gt;</c> directly.
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
}
