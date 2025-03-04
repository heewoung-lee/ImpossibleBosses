using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInitalizeNGO : NetworkBehaviourBase
{
    enum Transforms
    {
        Interaction
    }

    Transform _interactionTr;

    protected override void AwakeInit()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            Bind<Transform>(typeof(Transforms));
            _interactionTr = Get<Transform>((int)Transforms.Interaction);
            Managers.GameManagerEx.SetPlayer(gameObject);
            SetOwnerPlayerADD_Module();
            Managers.SocketEventManager.DonePlayerSpawnEvent?.Invoke(gameObject);
        }
    }
    [ServerRpc]
    public void SetNameServerRPC()
    {
      
    }





    protected override void StartInit()
    {

    }

    public void SetOwnerPlayerADD_Module()
    {
        gameObject.name = "Owner";
        //PlayerInput input = gameObject.GetOrAddComponent<PlayerInput>();
        //input.actions = Managers.ResourceManager.Load<InputActionAsset>("InputData/GameInputActions");
        gameObject.GetOrAddComponent<PlayerInput>();
        gameObject.GetOrAddComponent<PlayerStats>();
        gameObject.GetOrAddComponent<PlayerController>();
        gameObject.GetOrAddComponent<Module_Player_TextureCamera>();
        gameObject.GetOrAddComponent<Module_HP_Bar>();
        gameObject.GetOrAddComponent<Module_Damage_Text>();
        gameObject.GetOrAddComponent<Module_UI_BufferBar>();
        gameObject.GetOrAddComponent<Module_UI_ConsumableBar>();
        gameObject.GetOrAddComponent<Module_UI_ItemDragImage>();
        gameObject.GetOrAddComponent<Module_UI_Player_Inventory>();
        gameObject.GetOrAddComponent<Module_UI_PlayerInfo>();
        gameObject.GetOrAddComponent<Module_UI_SkillBar>();
        gameObject.GetOrAddComponent<Module_UI_Description>();
        gameObject.GetOrAddComponent<Module_MainCamera_CinemachineBrain>();
        gameObject.GetOrAddComponent<Module_Player_AnimInfo>();
        gameObject.GetOrAddComponent<Module_UI_Player_TestButton>();
        gameObject.GetOrAddComponent<Module_Fighter_Class>();//TODO: 우선 하드코딩, 플레이어의 직업에 맞는 클래스 넣어야함
        _interactionTr.GetOrAddComponent<Module_Player_Interaction>();
    }
}
