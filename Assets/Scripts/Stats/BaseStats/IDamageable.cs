using UnityEngine;

public interface IDamageable
{
    public void OnAttacked(IAttackRange attacker,int ?damage = null);
}