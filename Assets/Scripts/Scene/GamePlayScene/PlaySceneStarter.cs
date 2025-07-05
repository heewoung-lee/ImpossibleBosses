using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using Module.UI_Module;
using Scene.CommonInstaller;
using UI.Scene.SceneUI;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Scene.GamePlayScene
{
    public class PlaySceneStarter : ISceneStarter
    {
        [Inject] private ISceneSpawnBehaviour _sceneSpawnBehaviour;
        [Inject] private IUISceneManager _uiSceneManager;
        [Inject] private IInstantiate _instantiator;
        [Inject] ISceneMover _sceneMover;
        
        private UILoading _uiLoadingScene;
        private GamePlaySceneLoadingProgress _gamePlaySceneLoadingProgress;
        private PlayScene _playScene;

        public void SceneStart()
        {
            _playScene = Object.FindAnyObjectByType<PlayScene>();


            _sceneSpawnBehaviour.Init();
            _sceneSpawnBehaviour.SpawnObj();
            _instantiator.GetOrAddComponent<InGameUIModule>(_playScene.gameObject);
            _uiLoadingScene = _uiSceneManager.GetOrCreateSceneUI<UILoading>();
            _gamePlaySceneLoadingProgress =_instantiator.GetOrAddComponent<GamePlaySceneLoadingProgress>(_uiLoadingScene.gameObject);


        }
    }
}
