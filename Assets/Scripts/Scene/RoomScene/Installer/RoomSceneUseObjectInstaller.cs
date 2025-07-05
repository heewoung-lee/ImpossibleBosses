using System.Collections.Generic;
using NetWork.NGO;
using NetWork.NGO.UI;
using Scene.CommonInstaller;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scene.RoomScene.Installer
{
    public class RoomSceneUseObjectInstaller : BindNgoHandlerInstaller
    {
        List<GameObject> _needBindHandlerList;
        public override void InstallBindings()
        {
            _needBindHandlerList = new List<GameObject>(10);
            
            GameObject characterSelectRect = Resources.Load<GameObject>("Prefabs/NGO/NGOUICharacterSelectRect");
            Container.Bind<IFactory<CharacterSelectorNgo>>().To<CharacterSelectorNgo.CharacterSelectorNgoFactory>()
                .AsSingle().WithArguments(characterSelectRect);
            
            _needBindHandlerList.Add(characterSelectRect);


            GameObject ngoUIRootCharacterSelect = Resources.Load<GameObject>("Prefabs/NGO/NGOUIRootChracterSelect");
            Container.Bind<IFactory<NgoUIRootCharacterSelect>>()
                .To<NgoUIRootCharacterSelect.NgoUIRootCharacterSelectFactory>().AsSingle()
                .WithArguments(ngoUIRootCharacterSelect);
            
            _needBindHandlerList.Add(ngoUIRootCharacterSelect);
        }


        public override void Start()
        {
            base.Start();
            InputBindNgoHandler(_needBindHandlerList);
        }
    }
    
    
}
