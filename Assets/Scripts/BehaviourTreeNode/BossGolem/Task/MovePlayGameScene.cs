using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using GameManagers;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class MovePlayGameScene : Action
    {
        private NGO_MoveDownTownBehaviour _ngoMoveDownTownBehaviour;
        private NGO_Stage_Timer_Controller _ngoStageTimerController;
        private BehaviorTree _tree;
        private MoveSceneController _sceneMoverController;
        private ISceneMover _sceneMover;
        public override void OnStart()
        {
            _ngoMoveDownTownBehaviour = Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/NGO_MoveDownTownBehaviour").GetComponent<NGO_MoveDownTownBehaviour>();
            _ngoStageTimerController = Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller").GetComponent<NGO_Stage_Timer_Controller>();
            _sceneMoverController = ((ISceneController)Managers.SceneManagerEx.GetCurrentScene).SceneMoverController;//씬 무버가 없다면 오류뜨도록 설계
            _ngoStageTimerController.UI_Stage_Timer.OnTimerCompleted += _sceneMoverController.ISceneBehaviour.nextscene.MoveScene;
            _tree = Owner.GetComponent<BehaviorTree>();
            base.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (_tree == null) return TaskStatus.Running;
            _tree.DisableBehavior(); // 내부적으로 정리하면서 비활성화
            Managers.RelayManager.DeSpawn_NetWorkOBJ(_ngoMoveDownTownBehaviour.gameObject);
            return TaskStatus.Success;
        }
    }
}
