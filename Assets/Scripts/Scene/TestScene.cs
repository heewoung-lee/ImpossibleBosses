using GameManagers;
using Scene.GamePlayScene;
using UnityEngine;
using Util;

namespace Scene
{
    public class TestScene : BaseScene
    {
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
            _player = Managers.GameManagerEx.Spawn("Prefabs/Player/Fighter");
        }
    }
}