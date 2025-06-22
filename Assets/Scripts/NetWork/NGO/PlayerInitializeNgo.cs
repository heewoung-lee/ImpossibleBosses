using System;
using System.Collections.Generic;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using Module.CameraModule;
using Module.PlayerModule;
using Module.PlayerModule.PlayerClassModule;
using NetWork.BaseNGO;
using Player;
using Stats;
using UI.Scene.Interface;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Util;
using Zenject;

namespace NetWork.NGO
{
    public class PlayerInitializeNgo : NetworkBehaviourBase, ISceneChangeBehaviour
    {
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] GameManagerEx _gameManagerEx;
        
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
                _gameManagerEx.SetPlayer(gameObject);
                SetOwnerPlayerADD_Module();
            }
            Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += SetParentPosition;
        }
        private void SetParentPosition(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (Managers.RelayManager.NetworkManagerEx.IsHost == false)
                return;

            if (loadSceneMode != LoadSceneMode.Single)
                return;

            transform.SetParent(Managers.RelayManager.NgoRoot.transform);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted -= SetParentPosition;
        }
        protected override void StartInit()
        {

        }

        public void SetOwnerPlayerADD_Module()
        {
            gameObject.name = "OnwerPlayer";
            gameObject.GetComponent<PlayerStats>().enabled = true;
            gameObject.AddComponent<PlayerInput>();
            PlayerController controller = gameObject.AddComponent<PlayerController>();
            gameObject.AddComponent<ModuleMainCameraCinemachineBrain>();
            gameObject.AddComponent<ModulePlayerAnimInfo>();
            gameObject.AddComponent<ModulePlayerTextureCamera>();
            gameObject.AddComponent(GetPlayerModuleClass(Managers.RelayManager.ChoicePlayerCharacter));
            _interactionTr.AddComponent<ModulePlayerInteraction>();
            SetPlayerLayerMask();
      
            RuntimeAnimatorController ownerPlayerAnimController = _resourcesLoader.Load<RuntimeAnimatorController>($"Art/Player/AnimData/Animation/{Managers.RelayManager.ChoicePlayerCharacter}Controller");
            gameObject.GetComponent<Animator>().runtimeAnimatorController = ownerPlayerAnimController;
            _gameManagerEx.OnPlayerSpawnWithControllerModule(controller);
        }


        private Type GetPlayerModuleClass(Define.PlayerClass playerClass)
        {
            switch (playerClass)
            {
                case Define.PlayerClass.Archer:
                    return typeof(ModuleAcherClass); 
                case Define.PlayerClass.Fighter:
                    return typeof(ModuleFighterClass);
                case Define.PlayerClass.Mage:
                    return typeof(ModuleMageClass);
                case Define.PlayerClass.Monk:
                    return typeof(ModuleMonkClass);
                case Define.PlayerClass.Necromancer:
                    return typeof(ModuleNecromancerClass);

                default: return null;
            }
        }
        private void SetPlayerLayerMask()
        {
            LayerMask playerMask = LayerMask.NameToLayer("Player");
            gameObject.layer = playerMask;
            foreach (Transform childtr in gameObject.transform)
            {
                if (childtr.TryGetComponent(out ModulePlayerLayerField moduleField))
                {
                    moduleField.gameObject.layer = playerMask;
                    SetPlayerLayerMask(moduleField.transform, playerMask);
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

        public void OnBeforeSceneUnload()
        {
            if (IsOwner == false)
                return;
            
            gameObject.transform.SetParent(null, false);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SetForcePositionFromNetworkRpc(Vector3 position)
        {
            if (IsOwner == false)
                return;

            GetComponent<NavMeshAgent>().ResetPath();//플레이어가 이동중이라면 경로를 없앤다
            PlayerController controller = GetComponent<PlayerController>();// 상태를 IDLE로 강제로 바꾼다
            controller.CurrentStateType = controller.BaseIDleState;
            GetComponent<NetworkTransform>().Teleport(position,transform.rotation,transform.localScale);
            //포지션을 호스트가 바꾸는데 NavMesh에 대한 포지션만 변경하므로 NEtwork에는 업데이트가 안될 수 도 있기에
            //각자가 네트워크에서 포지션을 업데이트 해준다. 캐싱은 필요없음 씬전환시에만 호출 해서 쓸거기 때문에 캐싱은 안함
        }
    }
}
