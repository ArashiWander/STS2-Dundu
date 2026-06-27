using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DundunDudu.DundunDuduCode.Cards;
using DundunDudu.DundunDuduCode.Extensions;
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

    // Real basic starter: 4x Strike + 4x Defend + 1x 灵巧 (the Dexterity enabler, agile identity). The 17 cheap
    // cards + rares live in DuduCardPool and come from card rewards / the in-game `card` console — not pre-loaded
    // into the opening hand. (Reverted from the earlier sampler deck for the same reason as 墩墩.)
    public override IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> StartingDeck =>
    [
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

    // Thorough placeholder: use our generic placeholder select/icon/map textures (NOT Silent's face), and drop
    // the borrowed Silent-themed select background and card trail (null = base default, safe). The combat model
    // (CustomVisualPath via PlaceholderID "silent") is intentionally kept; real art swaps in later.
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

    // Select-screen background + card trail: inherit PlaceholderCharacterModel's (char_select_bg_silent, a real
    // existing scene). Returning null made the game look for a char_select_bg_dundundudu-dudu.tscn we don't ship
    // → load [ERROR]. Ship a custom bg scene later (real-art phase) to drop the borrowed one.
}
