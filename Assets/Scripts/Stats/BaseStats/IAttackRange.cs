using UnityEngine;

public interface IAttackRange
{
    float ViewAngle { get; }
    float ViewDistance { get; }
    Transform Owner_Transform { get; }
    LayerMask TarGetLayer { get; }

}