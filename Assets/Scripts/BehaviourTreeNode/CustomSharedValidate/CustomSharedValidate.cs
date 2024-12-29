using BehaviorDesigner.Runtime;
using DTT.AreaOfEffectRegions;

[System.Serializable]
public class SharedProjector : SharedVariable<Indicator_Controller>
{
    public static implicit operator SharedProjector(Indicator_Controller value)
    {
        return new SharedProjector { Value = value };
    }
}