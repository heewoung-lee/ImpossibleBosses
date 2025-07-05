using GameManagers;
using Scene.GamePlayScene;
using UnityEngine;
using Util;
using Zenject;

namespace Scene
{
    public class TestScene : BaseScene
    {
        private GameObject _player;

        public override Define.Scene CurrentScene => Define.Scene.Unknown;

        protected override void StartInit()
        {
            base.StartInit();
        }

        public override void Clear()
        {

        }

        protected override void AwakeInit()
        {
           
        }
    }
}