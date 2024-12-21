using BehaviorDesigner.Runtime;
using DTT.AreaOfEffectRegions;

[System.Serializable]
public class SharedArcRegionProjector : SharedVariable<ArcRegionProjector>
{
    public static implicit operator SharedArcRegionProjector(ArcRegionProjector value)
    {
        return new SharedArcRegionProjector { Value = value };
    }
}