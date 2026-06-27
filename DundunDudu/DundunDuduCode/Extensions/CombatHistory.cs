using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace DundunDudu.DundunDuduCode.Extensions;

/// <summary>
/// Read-only combat-history queries (FeralPower's pattern). Not pure (touches CombatManager), so not unit-tested —
/// these are game-state reads, not formula logic. Shared by cards (舞台聚光灯) and powers (蓄力).
/// </summary>
public static class CombatHistory
{
    /// <summary>How many attacks <paramref name="owner"/> has played so far THIS turn.</summary>
    public static int AttacksPlayedThisTurn(Creature owner, ICombatState combatState)
        => CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count(e => e.CardPlay.Card.Type == CardType.Attack
                        && e.CardPlay.Card.Owner.Creature == owner
                        && e.HappenedThisTurn(combatState));

    /// <summary>How many cards of a given <paramref name="cost"/> <paramref name="owner"/> has played THIS turn.</summary>
    public static int CardsPlayedThisTurnWithCost(Creature owner, ICombatState combatState, int cost)
        => CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count(e => e.CardPlay.Card.Owner.Creature == owner
                        && e.CardPlay.Resources.EnergyValue == cost
                        && e.HappenedThisTurn(combatState));

    /// <summary>How many block-granting skills <paramref name="owner"/> has played THIS turn.</summary>
    public static int BlockSkillsPlayedThisTurn(Creature owner, ICombatState combatState)
        => CombatManager.Instance.History.Entries.OfType<CardPlayStartedEntry>()
            .Count(e => e.CardPlay.Card.Owner.Creature == owner
                        && e.CardPlay.Card.Type == CardType.Skill
                        && e.CardPlay.Card.GainsBlock
                        && e.HappenedThisTurn(combatState));
}
