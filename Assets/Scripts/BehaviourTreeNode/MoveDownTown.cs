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
            Managers.SceneManagerEx.NetworkLoadScene(Define.Scene.GamePlayScene, (asd) => { }, () => { });
            //void ClientLoadedEvent(ulong clientId)
            //{
            //    Managers.RelayManager.NGO_RPC_Caller.GetPlayerChoiceCharacterRpc(clientId);
            //}

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
