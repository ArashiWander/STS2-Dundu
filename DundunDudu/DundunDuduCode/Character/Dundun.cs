using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DundunDudu.DundunDuduCode.Cards;
using DundunDudu.DundunDuduCode.Extensions;
using DundunDudu.DundunDuduCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DundunDudu.DundunDuduCode.Character;

/// <summary>
/// 墩墩 — Phase 3 character skeleton. PlaceholderCharacterModel borrows the Ironclad combat visuals
/// (PlaceholderID = "ironclad") so the character is fully solo-playable with placeholder art, before any
/// custom animation work. Starter deck is all-deterministic and single-player safe (no cross-player logic;
/// that is Phase 5 after the live co-op spike PASSes).
/// </summary>
public class Dundun : PlaceholderCharacterModel
{
    public const string CharacterId = "Dundun";

    public static readonly Color Color = new("e8743b"); // 墩墩 warm orange (placeholder; tune in Phase 6)

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 75;
    public override int MaxEnergy => 3;

    // Real basic starter (5x Strike + 4x Defend) + 2x SulkTestCard so the 闷气 tier-scaling is testable solo.
    // The 12 cheap cards + rares live in DundunCardPool and come from card rewards / the in-game `card` console —
    // they are intentionally NOT pre-loaded into the opening hand. SulkTestCard is TEMP (remove after P1a verify).
    public override IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> StartingDeck =>
    [
        ModelDb.Card<BigButt>(),
        ModelDb.Card<BigButt>(),
        ModelDb.Card<BigButt>(),
        ModelDb.Card<BigButt>(),
        ModelDb.Card<BigButt>(),
        ModelDb.Card<Steadfast>(),
        ModelDb.Card<Steadfast>(),
        ModelDb.Card<Steadfast>(),
        ModelDb.Card<Steadfast>(),
        ModelDb.Card<SulkTestCard>(),
        ModelDb.Card<SulkTestCard>()
    ];

    // 墩墩's design starter relic is 小熊虫 (first-lethal save). Replaces the placeholder BurningBlood.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<WaterBear>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<DundunCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<DundunRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<DundunPotionPool>();

    // Placeholder select-screen / icon / map-marker art (from the Character template's charui assets).
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();

    // Select-screen background + card trail: inherit PlaceholderCharacterModel's (char_select_bg_{PlaceholderID},
    // a real existing scene). Returning null made the game look for char_select_bg_dundundudu-dundun.tscn (which
    // we don't ship) → load [ERROR]; inheriting the placeholder bg is part of the same borrow as the ironclad
    // combat visuals. Ship a custom bg scene later (real-art phase) to truly drop the borrowed one.
}
