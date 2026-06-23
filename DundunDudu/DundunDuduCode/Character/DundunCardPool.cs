using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using Godot;

namespace DundunDudu.DundunDuduCode.Character;

public class DundunCardPool : CustomCardPoolModel
{
    public override string Title => Dundun.CharacterId; // internal id, not a display name

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card-back tint (HSV shader over a base image). Left near-neutral for the skeleton; tuned in Phase 6.
    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => Dundun.Color;

    public override bool IsColorless => false;
}
