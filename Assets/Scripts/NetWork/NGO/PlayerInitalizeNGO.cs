using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.Animations;
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
            Managers.SocketEventManager.InvokeDonePlayerSpawnEvent(gameObject);
        }
    }

    protected override void StartInit()
    {

    }

    public void SetOwnerPlayerADD_Module()
    {
        gameObject.name = "OnwerPlayer";
        gameObject.GetComponent<PlayerStats>().enabled = true;
        gameObject.AddComponent<PlayerInput>();
        gameObject.AddComponent<PlayerController>();
        gameObject.AddComponent<Module_MainCamera_CinemachineBrain>();
        gameObject.AddComponent<Module_Player_AnimInfo>();
        gameObject.AddComponent<Module_Player_TextureCamera>();
        gameObject.AddComponent(GetPlayerModuleClass(Managers.RelayManager.ChoicePlayerCharacter));
        _interactionTr.AddComponent<Module_Player_Interaction>();
        SetPlayerLayerMask();
      
        RuntimeAnimatorController OwnerPlayerAnimController = Managers.ResourceManager.Load<RuntimeAnimatorController>($"Art/Player/AnimData/Animation/{Managers.RelayManager.ChoicePlayerCharacter}Controller");
        gameObject.GetComponent<Animator>().runtimeAnimatorController = OwnerPlayerAnimController; 
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
    private void SetPlayerLayerMask()
    {
        LayerMask playerMask = LayerMask.NameToLayer("Player");
        gameObject.layer = playerMask;
        foreach (Transform childtr in gameObject.transform)
        {
            if (childtr.TryGetComponent(out Module_Player_LayerField module_Field))
            {
                module_Field.gameObject.layer = playerMask;
                SetPlayerLayerMask(module_Field.transform, playerMask);
            }
        }
    }
    private void SetPlayerLayerMask(Transform setLayerMaskTr,LayerMask layerMask)
    {
        foreach (Transform childtr in setLayerMaskTr.transform)
        {
            childtr.gameObject.layer = layerMask;
            SetPlayerLayerMask(childtr, layerMask);
        }
    }
}