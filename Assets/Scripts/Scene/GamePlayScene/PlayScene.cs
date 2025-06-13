using System.Collections.Generic;
using GameManagers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scene.GamePlayScene
{
    public class PlayScene : BaseScene,ISkillInit, ISceneController
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
                _sceneController = new MoveSceneController(new MockUnitGamePlayScene(Define.PlayerClass.Fighter, _uiLoadingScene, isSoloTest));
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
            }
            else
            {
                _sceneController = new MoveSceneController(new UnitGamePlayScene());
                gameObject.AddComponent<MockUnitUIGamePlaySceneModule>();
                // gameObject.AddComponent<UI_GamePlaySceneModule>(); TODO: 빌드시 테스트 모듈 제거할것
            }
        }

        public void OnEnable()
        {
            Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += LoadPlayScene;
        }
        public void OnDisable()
        {
            var sceneManager = Managers.RelayManager?.NetworkManagerEx?.SceneManager;
            if (sceneManager != null)
            {
                sceneManager.OnLoadEventCompleted -= LoadPlayScene;
            }
        }
  
        private void LoadPlayScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
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
        

        }

   

        public override void Clear()
        {
        }

    }
}