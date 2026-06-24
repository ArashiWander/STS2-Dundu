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

    // TEMP "sampler" starting deck: one of every 嘟嘟 Cheap card (+ the Dexterity enabler 灵巧), so the new
    // design can be experienced immediately solo. Revert to a real basic starter for balance later.
    public override IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> StartingDeck =>
    [
        ModelDb.Card<DuduNimble>(),
        ModelDb.Card<Peckish>(),
        ModelDb.Card<Poke>(),
        ModelDb.Card<TwoBites>(),
        ModelDb.Card<GuardFood>(),
        ModelDb.Card<PretendShare>(),
        ModelDb.Card<ExtraServing>(),
        ModelDb.Card<HugBowl>(),
        ModelDb.Card<Appetizer>(),
        ModelDb.Card<EatAndWatch>(),
        ModelDb.Card<GrabDish>(),
        ModelDb.Card<Whirlwind>(),
        ModelDb.Card<Feast>(),
        ModelDb.Card<SweepBuffet>(),
        ModelDb.Card<EatAndGrab>(),
        ModelDb.Card<BigMeal>(),
        ModelDb.Card<Fearless>(),
        ModelDb.Card<RevengeEating>()
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

    public override string CustomCharacterSelectBg => null!;
    public override string CustomTrailPath => null!;
}
