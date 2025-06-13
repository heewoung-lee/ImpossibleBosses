using UnityEngine;

namespace Stats.BaseStats
{
    public interface IAttackRange
    {
        float ViewAngle { get; }
        float ViewDistance { get; }
        Transform OwnerTransform { get; }
        Vector3 AttackPosition { get; }
        LayerMask TarGetLayer { get; }

    }
}