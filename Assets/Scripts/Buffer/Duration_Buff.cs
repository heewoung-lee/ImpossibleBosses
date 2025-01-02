using UnityEngine;

public abstract class Duration_Buff : Buff_Modifier
{
    public abstract void RemoveStats(BaseStats stats, float value);
    public abstract Sprite BuffIconImage { get; }
}