using System.Collections.Generic;
using System.Threading.Tasks;
using GameManagers;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
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
    public class PlayScene : BaseScene, ISkillInit,ISceneTestMode,ISceneMultiMode
    {
        [Inject] private ISceneSpawnBehaviour _sceneSpawnBehaviour;
        [Inject] private IUISceneManager _uiSceneManager;
        public TestMode GetTestMode()
        {
            return testMode;
        }
        public MultiMode GetMultiTestMode()
        {
           return multiMode;
        }
        
        [SerializeField] private TestMode testMode = TestMode.Local;
        [SerializeField] private MultiMode multiMode = MultiMode.Solo;
        
        [SerializeField] public Define.PlayerClass playerableCharacter = Define.PlayerClass.Archer;

        public override Define.Scene CurrentScene => Define.Scene.GamePlayScene;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour => _sceneSpawnBehaviour;

        private UILoading _uiLoadingScene;
        public UILoading UILoadingScene => _uiLoadingScene;
        
        
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;


        protected override void AwakeInit()
        {
          
        }

        protected override void StartInit()
        {
            base.StartInit();
            _sceneSpawnBehaviour.Init();
            _sceneSpawnBehaviour.SpawnObj();
            
            
            _uiLoadingScene = _uiSceneManager.GetOrCreateSceneUI<UILoading>();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
        }


        public override void Clear()
        {
        }

    }
}