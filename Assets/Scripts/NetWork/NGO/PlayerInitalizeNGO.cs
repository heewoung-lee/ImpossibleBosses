using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

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
            Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += SetParentPosition;
        }
    }

    private void SetParentPosition(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
            return;

        if (loadSceneMode != LoadSceneMode.Single)
            return;

        //foreach (ulong clientId in clientsCompleted)
        //{
        //    if (Managers.RelayManager.NetworkManagerEx.ConnectedClients.TryGetValue(clientId, out Unity.Netcode.NetworkClient networkClient))
        //    {
        //        NetworkObject playerNetworkObject = networkClient.PlayerObject;

        //        if (playerNetworkObject != null)
        //        {
        //            if (playerNetworkObject.TryGetComponent(out PlayerStats stats))
        //            {
        //                stats.transform.SetParent(Managers.RelayManager.NGO_ROOT.transform);
        //                Debug.Log($"플레이어 {clientId}가 NGO_ROOT의 자식으로 설정되었습니다.");
        //            }
        //            else
        //            {
        //                Debug.Log($"클라이언트 {clientId}의 PlayerObject에서 PlayerStats 컴포넌트를 찾을 수 없습니다.");
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log($"클라이언트 {clientId}의 PlayerObject가 null입니다. (아직 스폰되지 않았거나 주 플레이어 오브젝트가 아닐 수 있습니다.)");
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log($"ConnectedClients에서 클라이언트 {clientId}를 찾을 수 없습니다. (이미 연결이 끊겼을 수 있습니다.)");
        //    }
        //}
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