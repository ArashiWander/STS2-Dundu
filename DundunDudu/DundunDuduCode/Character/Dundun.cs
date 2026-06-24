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

    // TEMP "sampler" starting deck: one of every 墩墩 Cheap card, so the new design can be experienced
    // immediately in a solo run. Revert to a real basic starter (e.g. 5x Strike + 5x Defend) for balance later.
    public override IEnumerable<MegaCrit.Sts2.Core.Models.CardModel> StartingDeck =>
    [
        ModelDb.Card<Grip>(),
        ModelDb.Card<NourishingSoup>(),
        ModelDb.Card<SteadyPunch>(),
        ModelDb.Card<Grapple>(),
        ModelDb.Card<Reserve>(),
        ModelDb.Card<Wellness>(),
        ModelDb.Card<DefensiveStep>(),
        ModelDb.Card<Backhand>(),
        ModelDb.Card<SlowSimmer>(),
        ModelDb.Card<Hedge>(),
        ModelDb.Card<RiskControl>(),
        ModelDb.Card<Counterplay>(),
        ModelDb.Card<BarbedArmor>(),
        ModelDb.Card<SustainedDefense>(),
        ModelDb.Card<DoubleKick>(),
        ModelDb.Card<IronWall>(),
        ModelDb.Card<StillWater>(),
        ModelDb.Card<GrandFeast>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<BurningBlood>()];

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

    // Thorough placeholder: drop the borrowed Ironclad-themed select background and card trail (null = the
    // CustomCharacterModel base default, so it's safe — the game shows its neutral default instead). The combat
    // model (CustomVisualPath via PlaceholderID) is intentionally kept; real art swaps in later.
    public override string CustomCharacterSelectBg => null!;
    public override string CustomTrailPath => null!;
}
