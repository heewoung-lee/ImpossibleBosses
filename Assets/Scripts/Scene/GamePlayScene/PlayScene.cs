using System.Collections.Generic;
using GameManagers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene.GamePlayScene
{
    public class PlayScene : BaseScene, ISkillInit, ISceneController
    {
        public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private MoveSceneController _sceneController;
        MoveSceneController ISceneController.SceneMoverController => _sceneController;

        [SerializeField] bool isTest = false;
        [SerializeField] bool isSoloTest = false;


        protected override void AwakeInit()
        {
            if (isTest == true)
            {
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
            }
            else
            {
                _sceneController = new MoveSceneController(new UnitGamePlayScene());
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
                Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += LoadPlayScene;
                // gameObject.AddComponent<UI_GamePlaySceneModule>(); TODO: 빌드시 테스트 모듈 제거할것
            }
        }

        //
        // TODO: 씬 컨트롤러 주입기에 대해서 생각 해볼것, 현재로써는 컨트롤러가 왜 존재하는지에 대한 이유를 찾지못함.
        //     차라리 외부 인스펙터에 의해 객체 주입을 받으면 어떻게 처리 할껀지 분기여부를 컨트롤러가 담당하고,
        //     씬에서는 공통된 비헤이비어를 호출하는게 맞다고 생각 즉 씬에서는 공통된 비헤이비어만 굴려주고
        // 각자의 씬컨트롤러에서 주입받은 객체에 대해 어떻게 처리할껀지 나눠주기
        
        public void OnDisable()
        {
            var sceneManager = Managers.RelayManager?.NetworkManagerEx?.SceneManager;
            if (sceneManager != null)
            {
                sceneManager.OnLoadEventCompleted -= LoadPlayScene;
            }
        }

        private void LoadPlayScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
            List<ulong> clientsTimedOut)
        {
            if (sceneName != Define.Scene.GamePlayScene.ToString())
                return;

            if (clientsCompleted.Contains(Managers.RelayManager.NetworkManagerEx.LocalClientId) is false)
                return;

            _sceneController.InitGamePlayScene();
            _sceneController.SpawnObj();
        }


        protected override void StartInit()
        {
            base.StartInit();
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
            if (isTest)
            {
                _sceneController =
                    new MoveSceneController(new MockUnitGamePlayScene(Define.PlayerClass.Fighter, _uiLoadingScene,
                        isSoloTest));
                _sceneController.InitGamePlayScene();
                _sceneController.SpawnObj();
            }
        }


        public override void Clear()
        {
        }
    }
}