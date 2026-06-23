using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace DundunDudu.DundunDuduCode;

//You're recommended but not required to keep all your code in this package and all your assets in the DundunDudu folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "DundunDudu"; //Logger + harmony names, and the res:// asset root.
    public const string ResPath = $"res://{ModId}"; //Asset root inside the .pck (must match manifest id / pck_name).

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
     
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}
