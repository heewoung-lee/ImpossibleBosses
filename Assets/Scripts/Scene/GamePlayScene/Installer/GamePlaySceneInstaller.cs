using UnityEngine;
using Zenject;

namespace Scene.GamePlayScene.Installer
{
    public class GamePlaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            PlayScene playScene = FindAnyObjectByType<PlayScene>();
            
        }
    }
}
