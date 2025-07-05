using System.Collections.Generic;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
using Scene.CommonInstaller;
using UI.Scene.SceneUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine.Serialization;
using Zenject;

namespace Scene.GamePlayScene
{
    public class PlayScene : BaseScene, ISkillInit,ISceneTestMode,ISceneMultiMode,ISceneSelectCharacter,IHasSceneMover
    {
        [Inject] private ISceneConnectOnline _sceneConnectOnline;
        [Inject] private ISceneStarter _gameplaySceneStarter;
        [Inject] private ISceneMover _sceneMover;

        public ISceneMover SceneMover => _sceneMover;
        public TestMode GetTestMode()
        {
            return testMode;
        }
        public MultiMode GetMultiTestMode()
        {
           return multiMode;
        }
        public Define.PlayerClass GetPlayerableCharacter()
        {
          return playerableCharacter;
        }
        
        [SerializeField] private TestMode testMode = TestMode.Local;
        [SerializeField] private MultiMode multiMode = MultiMode.Solo;
        [SerializeField] private Define.PlayerClass playerableCharacter = Define.PlayerClass.Archer;
        public const string PlayerableCharacterField = nameof(playerableCharacter);
        public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;
        private UILoading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        protected override void StartInit()
        {
            base.StartInit();
            _ = StartInitAsync();
        }
        
        private async Task StartInitAsync()
        {
            try
            {
                await _sceneConnectOnline.SceneConnectOnlineStart();
                _gameplaySceneStarter.SceneStart();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RoomPlayScene] 초기화 중 예외: {e}");
            }
        }
    }
}