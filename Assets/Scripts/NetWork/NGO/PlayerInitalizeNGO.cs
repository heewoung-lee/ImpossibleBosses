using System;
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
        Debug.Log(Managers.SceneManagerEx.GetCurrentScene);

        gameObject.name = "OnwerPlayer";
        //PlayerInput input = gameObject.GetOrAddComponent<PlayerInput>();
        //input.actions = Managers.ResourceManager.Load<InputActionAsset>("InputData/GameInputActions");
        gameObject.AddComponent<PlayerInput>();
        gameObject.AddComponent<PlayerStats>();
        gameObject.AddComponent<PlayerController>();
        gameObject.AddComponent<Module_Player_TextureCamera>();
        gameObject.AddComponent<Module_HP_Bar>();
        gameObject.AddComponent<Module_Damage_Text>();
        gameObject.AddComponent<Module_UI_BufferBar>();
        gameObject.AddComponent<Module_UI_ConsumableBar>();
        gameObject.AddComponent<Module_UI_ItemDragImage>();
        gameObject.AddComponent<Module_UI_Player_Inventory>();
        gameObject.AddComponent<Module_UI_PlayerInfo>();
        gameObject.AddComponent<Module_UI_SkillBar>();
        gameObject.AddComponent<Module_UI_Description>();
        gameObject.AddComponent<Module_MainCamera_CinemachineBrain>();
        gameObject.AddComponent<Module_Player_AnimInfo>();
        gameObject.AddComponent<Module_UI_Player_TestButton>();
        gameObject.AddComponent(GetPlayerModuleClass(Managers.RelayManager.ChoicePlayerCharacter));
        _interactionTr.AddComponent<Module_Player_Interaction>();
    }


    private Type GetPlayerModuleClass(Define.PlayerClass playerclass)
    {
        switch (playerclass)
        {
            case Define.PlayerClass.Archer:
                return typeof(Module_Acher_Class); 
            case Define.PlayerClass.Fighter:
                return typeof(Module_Fighter_Class);
            case Define.PlayerClass.Mage:
                return typeof(Module_Mage_Class);
            case Define.PlayerClass.Monk:
                return typeof(Module_Monk_Class);
            case Define.PlayerClass.Necromancer:
                return typeof(Module_Necromancer_Class);

            default: return null;
        }
    }
}