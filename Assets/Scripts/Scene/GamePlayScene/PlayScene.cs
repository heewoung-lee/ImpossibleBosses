using System.Collections.Generic;
using GameManagers;
using Module.UI_Module;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine.Serialization;

namespace Scene.GamePlayScene
{
    public class PlayScene : BaseScene, ISkillInit, ISceneController
    {

        public enum TestMode
        {
           Local,
           Multi
        }

        public enum MultiMode
        {
            Solo,
            Multi
        }

        
        [SerializeField] public TestMode testMode = TestMode.Local;
        [SerializeField] public MultiMode multiMode = MultiMode.Solo;
        [SerializeField] public Define.PlayerClass playerableCharacter = Define.PlayerClass.Archer;
        
        
        public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;

        private UI_Loading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private MoveSceneController _sceneController;
        MoveSceneController ISceneController.SceneMoverController => _sceneController;

        

        protected override void AwakeInit()
        {
            _uiLoadingScene = Managers.UIManager.GetOrCreateSceneUI<UI_Loading>();
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
        }


        protected override void StartInit()
        {
            base.StartInit();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
            if (Managers.SceneManagerEx.IsNormalBoot == true)
            {
                _sceneController = new MoveSceneController(new UnitNetGamePlayScene());
                gameObject.AddComponent<InGameUIModule>();
                //노멀 모드를 주입해야함.
            }
            else
            {
                if (testMode == TestMode.Local) //로컬에서 테스트를 굴릴때
                {
                    _sceneController = new MoveSceneController(new UnitLocalGamePlayScene());
                }
                else if (testMode == TestMode.Multi)
                {
                    if (multiMode == MultiMode.Solo)
                    {
                        _sceneController =
                            new MoveSceneController(new MockUnitGamePlayScene(playerableCharacter, _uiLoadingScene,
                                true));
                    }
                    else if (multiMode == MultiMode.Multi)
                    {
                        _sceneController =
                            new MoveSceneController(new MockUnitGamePlayScene(playerableCharacter, _uiLoadingScene,
                                false));
                        //멀티모드인데 멀티인지
                    }
                    gameObject.AddComponent<MockUnitUIGamePlaySceneModule>(); //테스트용 UI삽입
                }
            }

            _sceneController.InitGamePlayScene();
        }


        public override void Clear()
        {
        }
    }
}