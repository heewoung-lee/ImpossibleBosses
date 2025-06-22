using GameManagers;
using Scene.GamePlayScene;
using UnityEngine;
using Util;
using Zenject;

namespace Scene
{
    public class TestScene : BaseScene
    {
        [Inject] GameManagerEx _gameManagerEx;
        private GameObject _player;

        public override Define.Scene CurrentScene => Define.Scene.Unknown;
        public override ISceneSpawnBehaviour SceneSpawnBehaviour { get; }

        protected override void StartInit()
        {
            base.StartInit();
        }

        public override void Clear()
        {

        }

        protected override void AwakeInit()
        {
            _player = _gameManagerEx.Spawn("Prefabs/Player/Fighter");
        }
    }
}