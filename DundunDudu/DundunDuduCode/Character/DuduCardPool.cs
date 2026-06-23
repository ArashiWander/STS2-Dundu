using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using Godot;

namespace DundunDudu.DundunDuduCode.Character;

public class DuduCardPool : CustomCardPoolModel
{
    public override string Title => Dudu.CharacterId; // internal id, not a display name

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    // Card-back tint (HSV shader). Hue shifted from 墩墩's (H=1) toward a cooler back. Placeholder; tune Phase 6.
    public override float H => 0.5f;
    public override float S => 1f;
    public override float V => 1f;

    public override Color DeckEntryCardColor => Dudu.Color;

    public override bool IsColorless => false;
}
