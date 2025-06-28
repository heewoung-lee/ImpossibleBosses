using System.Collections.Generic;
using GameManagers;
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
    public class PlayScene : BaseScene, ISkillInit
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
        [Inject] private ISceneSpawnBehaviour _sceneSpawnBehaviour;
        [Inject] private UIManager _uiManager;

        
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
            
            
            _uiLoadingScene = _uiManager.GetOrCreateSceneUI<UILoading>();
            _gamePlaySceneLoadingProgress = _uiLoadingScene.AddComponent<GamePlaySceneLoadingProgress>();
        }


        public override void Clear()
        {
        }
    }
}