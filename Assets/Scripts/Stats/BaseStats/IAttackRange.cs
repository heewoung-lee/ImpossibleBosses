using UnityEngine;

public interface IAttackRange
{
    float ViewAngle { get; }
    float ViewDistance { get; }
    Transform Owner_Transform { get; }
    Vector3 AttackPosition { get; }
    LayerMask TarGetLayer { get; }

}