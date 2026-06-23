using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DundunDudu.DundunDuduCode.Character;

/// <summary>
/// 嘟嘟 — Phase 4 character skeleton. Theme: agility / combo / low-cost setup, complementing 墩墩's tanky base.
/// PlaceholderID "silent" borrows Silent's lighter/agile combat visuals (contrast vs 墩墩's ironclad), so 嘟嘟
/// is fully solo-playable with placeholder art. Squishier (65 HP) with a Dexterity-setup signature.
///
/// All-deterministic, single-player safe. ZERO cross-player logic — synergy hooks are Phase 5, gated on the
/// live co-op spike PASS (Phase 2). 墩墩 is untouched.
/// </summary>
public class Dudu : PlaceholderCharacterModel
{
    public const string CharacterId = "Dudu";

    public static readonly Color Color = new("2ec4b6"); // teal — deliberately contrasts 墩墩's warm orange

    public override string PlaceholderID => "silent";
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 65;
    public override int MaxEnergy => 3;

    public override IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> StartingDeck =>
    [
        ModelDb.Card<DuduStrike>(),
        ModelDb.Card<DuduStrike>(),
        ModelDb.Card<DuduStrike>(),
        ModelDb.Card<DuduStrike>(),
        ModelDb.Card<DuduStrike>(),
        ModelDb.Card<DuduDefend>(),
        ModelDb.Card<DuduDefend>(),
        ModelDb.Card<DuduDefend>(),
        ModelDb.Card<DuduDefend>(),
        ModelDb.Card<DuduNimble>()
    ];

    // RingOfTheSnake (Silent's starter: draw extra on turn 1) fits the low-cost "setup" feel. Placeholder.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<RingOfTheSnake>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<DuduCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DuduRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DuduPotionPool>();

    // No charui art overrides: inherits Silent's placeholder select/icon/map art via PlaceholderID,
    // which gives a distinct silhouette from 墩墩 (whose overrides use the generic template art).
}
