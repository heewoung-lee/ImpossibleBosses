using GameManagers.Interface.Resources_Interface;
using NetWork.NGO;
using UnityEngine;
using Zenject;

namespace Scene.RoomScene.Installer
{
    public class RoomSceneGameObjectInstaller : MonoInstaller
    {
        [Inject] private IResourcesLoader _loder;
        
        private GameObject _nGouiCharacterSelectRect;
        public override void InstallBindings()
        {
            _nGouiCharacterSelectRect = _loder.Load<GameObject>("Prefabs/NGO/NGOUICharacterSelectRect");
            
            
            
            Container.BindFactory<CharacterSelectorNgo,CharacterSelectorNgo.Factory>().FromComponentInNewPrefab(_nGouiCharacterSelectRect);
        }
    }
}
