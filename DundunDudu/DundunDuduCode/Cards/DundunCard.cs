using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using DundunDudu.DundunDuduCode.Character;
using DundunDudu.DundunDuduCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace DundunDudu.DundunDuduCode.Cards;

/// <summary>
/// Base class for 墩墩's cards: registers into <see cref="DundunCardPool"/> and resolves card art from the
/// mod's resources (falls back to placeholder card.png when a specific portrait is absent).
/// </summary>
[Pool(typeof(DundunCardPool))]
public abstract class DundunCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
