using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using GameManagers;
using NetWork.NGO.Scene_NGO;
using Scene;

namespace BehaviourTreeNode.BossGolem.Task
{
    public class MovePlayGameScene : Action
    {
        private NgoMoveDownTownBehaviour _ngoMoveDownTownBehaviour;
        private NgoStageTimerController _ngoStageTimerController;
        private BehaviorTree _tree;
        private ISceneMover _sceneMover;
        public override void OnStart()
        {
            _ngoMoveDownTownBehaviour = Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/NGO_MoveDownTownBehaviour").GetComponent<NgoMoveDownTownBehaviour>();
            _ngoStageTimerController = Managers.RelayManager.SpawnNetworkObj("Prefabs/NGO/Scene_NGO/NGO_Stage_Timer_Controller").GetComponent<NgoStageTimerController>();
            _sceneMover = Managers.SceneManagerEx.GetCurrentScene.SceneSpawnBehaviour.Nextscene;//씬 무버가 없다면 오류뜨도록 설계
            _ngoStageTimerController.UIStageTimer.OnTimerCompleted += _sceneMover.MoveScene;
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
