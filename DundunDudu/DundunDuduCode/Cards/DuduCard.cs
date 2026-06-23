using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using DundunDudu.DundunDuduCode.Character;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>Base class for 嘟嘟's cards: registers into <see cref="DuduCardPool"/> and resolves card art.</summary>
[Pool(typeof(DuduCardPool))]
public abstract class DuduCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
