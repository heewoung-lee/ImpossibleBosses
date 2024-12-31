using Microsoft.Unity.VisualStudio.Editor;

public abstract class Buff_Modifier
{
    public abstract string Buffname { get; }
    public abstract StatType StatType { get; }
    public abstract void ApplyStats(BaseStats stats, float value);

}