using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MovePlayGameScene : Action
{
    NGO_MoveDownTownBehaviour _ngo_MoveDownTownBehaviour;
    NGO_Stage_Timer_Controller _ngo_Timer_Controller;
    BehaviorTree _tree;
    MoveSceneController _sceneMoverController;
    ISceneMover _sceneMover;
    public override void OnStart()
    {
        _ngo_MoveDownTownBehaviour = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/NGO_MoveDownTownBehaviour").GetComponent<NGO_MoveDownTownBehaviour>();
        _ngo_Timer_Controller = Managers.RelayManager.SpawnNetworkOBJ("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller").GetComponent<NGO_Stage_Timer_Controller>();
        _sceneMoverController = (Managers.SceneManagerEx.GetCurrentScene as ISceneController).SceneMoverController;
        _ngo_Timer_Controller.UI_Stage_Timer.OnTimerCompleted += _sceneMoverController.ISceneBehaviour.nextscene.MoveScene;
        _tree = Owner.GetComponent<BehaviorTree>();
        base.OnStart();
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
