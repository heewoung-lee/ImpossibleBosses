using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MoveDownTown : Action
{
    NGO_MoveDownTownBehaviour _ngo_MoveDownTownBehaviour;
    NGO_Stage_Timer_Controller _ngo_Timer_Controller;
    BehaviorTree _tree; 
    public override void OnStart()
    {
        _ngo_MoveDownTownBehaviour = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/NGO_MoveDownTownBehaviour").GetComponent<NGO_MoveDownTownBehaviour>();
        _ngo_Timer_Controller = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller").GetComponent<NGO_Stage_Timer_Controller>();
        _ngo_Timer_Controller.UI_Stage_Timer.OnTimerCompleted += MoveToDownTown;
        _tree = Owner.GetComponent<BehaviorTree>();
        base.OnStart();

        void MoveToDownTown()//호스트에게만 실행됨.
        {
            Managers.RelayManager.NetworkManagerEx.NetworkConfig.EnableSceneManagement = true;
            Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene, ClientLoadedEvent, () => { });
            void ClientLoadedEvent(ulong clientId)
            {
                Debug.Log($"{clientId} 플레이어 로딩 완료");

                foreach (NetworkObject clicentNgoObj in Managers.RelayManager.NetworkManagerEx.SpawnManager.SpawnedObjectsList)
                {
                    if (clicentNgoObj.OwnerClientId != clientId)
                    {
                        continue;
                    }
                    if(clicentNgoObj.TryGetComponent(out PlayerStats playerStats) == true)
                    {
                        Debug.Log($"{clientId}플레이어 찾았다");
                        playerStats.transform.position = new Vector3(clientId,0,0);
                        break;
                    }
                }
                //TODO: 플레이어 스폰위치 조정
                //TODO: 시계 UI 없애야함
                //TODO: 각 플레이어들의 스폰위치를 정해줘야함.


            }

            //void AllPlayerLoadedEvent()
            //{
            //    PlayScene playScene = null;
            //    foreach (BaseScene scene in Managers.SceneManagerEx.GetCurrentScenes)
            //    {
            //        if (scene is PlayScene outPlayScene)
            //        {
            //            playScene = outPlayScene;
            //            break;
            //        }
            //    }
            //    playScene.Init_NGO_PlayScene_OnHost();
            //}
        }

    }

    public override TaskStatus OnUpdate()
    {
        if (_tree != null)
        {
            _tree.DisableBehavior(); // 내부적으로 정리하면서 비활성화
            Managers.RelayManager.DeSpawn_NetWorkOBJ(_ngo_MoveDownTownBehaviour.gameObject);
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
