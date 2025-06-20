using BehaviorDesigner.Runtime;
using VFX;

[System.Serializable]
public class SharedProjector : SharedVariable<NgoIndicatorController>
{
    public static implicit operator SharedProjector(NgoIndicatorController value)
    {
        return new SharedProjector { Value = value };
    }
}