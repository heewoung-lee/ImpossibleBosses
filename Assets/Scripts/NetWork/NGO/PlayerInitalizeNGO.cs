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
            SetOwnerPlayerADD_Module();
            Managers.GameManagerEx.SetPlayer(gameObject);


            Managers.SocketEventManager.PlayerSpawnInitalize?.Invoke(gameObject);
        }
    }


    protected override void StartInit()
    {

    }

    public void SetOwnerPlayerADD_Module()
    {
        _interactionTr.AddComponent<Module_Player_Interaction>();
        //PlayerInput input = gameObject.GetOrAddComponent<PlayerInput>();
        //input.actions = Managers.ResourceManager.Load<InputActionAsset>("InputData/GameInputActions");
        
        gameObject.GetOrAddComponent<PlayerInput>();
        gameObject.GetOrAddComponent<PlayerStats>();
        gameObject.GetOrAddComponent<PlayerController>();
        gameObject.GetOrAddComponent<Module_PlayerRenderTextureCamara>();
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
    }
}
