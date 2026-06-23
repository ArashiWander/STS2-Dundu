using BaseLib.Abstracts;
using DundunDudu.DundunDuduCode.Extensions;
using Godot;

namespace DundunDudu.DundunDuduCode.Character;

public class DundunPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => Dundun.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
