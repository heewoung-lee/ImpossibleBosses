using System;
using Unity.Netcode;
using UnityEngine;

public class NGO_Dust_Initalize : NGO_PoolingInitalize_Base
{
    public override string PoolingNGO_PATH => "Prefabs/Paticle/AttackEffect/Dust_Paticle";

    public override int PoolingCapacity => 100;

}
