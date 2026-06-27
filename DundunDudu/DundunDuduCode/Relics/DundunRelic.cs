using BaseLib.Abstracts;
using BaseLib.Utils;
using DundunDudu.DundunDuduCode.Character;

namespace DundunDudu.DundunDuduCode.Relics;

/// <summary>
/// Base for 墩墩's companion (玩偶) relics: registers into <see cref="DundunRelicPool"/>. Placeholder icons are
/// wired once in MainFile via BaseLib's RelicImageOverridePatch (the relic-image API). Each relic sets its own
/// Rarity + hooks. All deterministic + single-player safe — no cross-player logic.
/// </summary>
[Pool(typeof(DundunRelicPool))]
public abstract class DundunRelic : CustomRelicModel
{
}
