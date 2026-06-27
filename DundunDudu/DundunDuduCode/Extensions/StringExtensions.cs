using Godot;

namespace DundunDudu.DundunDuduCode.Extensions;

// Utilities to build res:// asset paths (with placeholder fallbacks), mirroring the Character template.
// NOTE: res:// is a Godot virtual path and MUST use '/' separators. Path.Join() emits '\' on Windows
// (res://DundunDudu\images\... ) which Godot's asset cache fails to match — so build these by hand with '/'.
public static class StringExtensions
{
    public static string ImagePath(this string path) => $"{MainFile.ResPath}/images/{path}";

    public static string CardImagePath(this string path)
    {
        string full = $"{MainFile.ResPath}/images/card_portraits/{path}";
        if (ResourceLoader.Exists(full)) return full;
        MainFile.Logger.Info("Could not find card image path: " + full);
        return $"{MainFile.ResPath}/images/card_portraits/card.png";
    }

    public static string BigCardImagePath(this string path)
    {
        string full = $"{MainFile.ResPath}/images/card_portraits/big/{path}";
        if (ResourceLoader.Exists(full)) return full;
        MainFile.Logger.Info("Could not find big card image path: " + full);
        return $"{MainFile.ResPath}/images/card_portraits/big/card.png";
    }

    public static string PowerImagePath(this string path)
    {
        string full = $"{MainFile.ResPath}/images/powers/{path}";
        if (ResourceLoader.Exists(full)) return full;
        MainFile.Logger.Info("Could not find power image path: " + full);
        return $"{MainFile.ResPath}/images/powers/power.png";
    }

    public static string RelicImagePath(this string path)
    {
        string full = $"{MainFile.ResPath}/images/relics/{path}";
        if (ResourceLoader.Exists(full)) return full;
        MainFile.Logger.Info("Could not find relic image path: " + full);
        return $"{MainFile.ResPath}/images/relics/relic.png";
    }

    public static string CharacterUiPath(this string path) => $"{MainFile.ResPath}/images/charui/{path}";
}
