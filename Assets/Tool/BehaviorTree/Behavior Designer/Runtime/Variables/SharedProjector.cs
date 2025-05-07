using BehaviorDesigner.Runtime;

[System.Serializable]
public class SharedProjector : SharedVariable<NGO_Indicator_Controller>
{
    public static implicit operator SharedProjector(NGO_Indicator_Controller value)
    {
        return new SharedProjector { Value = value };
    }
}