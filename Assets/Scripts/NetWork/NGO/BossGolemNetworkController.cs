using BehaviorDesigner.Runtime;
using System;
using Unity.Netcode;

public class BossGolemNetworkController : NetworkBehaviourBase
{
    NetworkVariable<float> _boss_Attack1_AnimSpeed = new NetworkVariable<float>
        (0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private BossGolemController _controller;
    private float _animLength = 0f;
    private Action<float> _boss_Attack1_Anum_Event;


    public event Action<float> Boss_Attack1_Anim_Event
    {
        add
        {
            UniqueEventRegister.AddSingleEvent(ref _boss_Attack1_Anum_Event, value);
        }
        remove
        {
            UniqueEventRegister.RemovedEvent(ref _boss_Attack1_Anum_Event, value);
        }

    } 

    protected override void AwakeInit()
    {
        _controller = GetComponent<BossGolemController>();
        _animLength = Utill.GetAnimationLength("Anim_Attack1", _controller.Anim);
    }

    protected override void StartInit()
    {
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Managers.GameManagerEx.SetBossMonster(gameObject);

        if (IsHost == false)
        {
            GetComponent<BossController>().enabled = false;
            GetComponent<BossGolemStats>().enabled = false;
            GetComponent<BehaviorTree>().enabled = false;
        }
    }
}
