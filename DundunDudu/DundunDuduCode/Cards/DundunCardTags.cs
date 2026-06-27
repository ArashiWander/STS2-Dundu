using MegaCrit.Sts2.Core.Models;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// 墩墩's mod-internal card-tag system. NOTE: BaseLib 3.3.2 — the version we compile against — has NO
/// ModCardTagRegistry; that type lives inside YuWanCard's own assembly, not BaseLib (verified against the NuGet
/// and installed dlls), so we can't reference it. Instead we use a marker interface + enum (mirroring YuWanCard's
/// own IYuWanCard marker). These tags are only ever queried by 墩墩's own cards/relics, so a mod-internal scheme
/// is sufficient. 猪饲料 cards carry Snack; 麦霸 cards carry Karaoke (applied in P3b).
/// </summary>
public enum DundunCardTag { Snack, Karaoke }

/// <summary>A 墩墩 card carrying one or more <see cref="DundunCardTag"/>s.</summary>
public interface ITaggedDundunCard
{
    IReadOnlySet<DundunCardTag> DundunTags { get; }
}

public static class DundunCardTags
{
    public static bool Has(CardModel card, DundunCardTag tag) => card is ITaggedDundunCard t && t.DundunTags.Contains(tag);
    public static bool IsSnack(CardModel card) => Has(card, DundunCardTag.Snack);
    public static bool IsKaraoke(CardModel card) => Has(card, DundunCardTag.Karaoke);
}
