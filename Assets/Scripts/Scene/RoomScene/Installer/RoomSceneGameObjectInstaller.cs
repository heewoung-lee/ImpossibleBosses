using GameManagers.Interface.Resources_Interface;
using NetWork.NGO;
using NetWork.NGO.UI;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scene.RoomScene.Installer
{
    public class RoomSceneGameObjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IFactory<CharacterSelectorNgo>>().To<CharacterSelectorNgo.CharacterSelectorNgoFactory>().AsSingle();
            
            Container.Bind<IFactory<NgoUIRootCharacterSelect>>().To<NgoUIRootCharacterSelect.NgoUIRootCharacterSelectFactory>().AsSingle();
            
        }
    }
}
