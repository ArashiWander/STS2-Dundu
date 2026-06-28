namespace DundunDudu.DundunDuduCode.Logic;

/// <summary>Pure 银色山泉 math — NO game types, unit-tested.</summary>
public static class SilverSpringMath
{
    /// <summary>银色山泉: 闷气 is halved; the removed half becomes that many 人工制品 (Artifact).</summary>
    public static int ArtifactFromSulking(int sulkStacks) => (sulkStacks < 0 ? 0 : sulkStacks) / 2;
}
