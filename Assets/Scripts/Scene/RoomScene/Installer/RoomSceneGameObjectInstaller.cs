using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using NetWork.NGO;
using NetWork.NGO.UI;
using Scene.CommonInstaller;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using ZenjectTool;

namespace Scene.RoomScene.Installer
{
    public class RoomSceneGameObjectInstaller : MonoInstaller
    {
        List<GameObject> prefabList;
        public override void InstallBindings()
        {
            prefabList = new List<GameObject>(10);
            
            GameObject characterSelectRect = Resources.Load<GameObject>("Prefabs/NGO/NGOUICharacterSelectRect");
            Container.Bind<IFactory<CharacterSelectorNgo>>().To<CharacterSelectorNgo.CharacterSelectorNgoFactory>()
                .AsSingle().WithArguments(characterSelectRect);
            
            prefabList.Add(characterSelectRect);


            GameObject ngoUIRootCharacterSelect = Resources.Load<GameObject>("Prefabs/NGO/NGOUIRootChracterSelect");
            Container.Bind<IFactory<NgoUIRootCharacterSelect>>()
                .To<NgoUIRootCharacterSelect.NgoUIRootCharacterSelectFactory>().AsSingle()
                .WithArguments(ngoUIRootCharacterSelect);
            
            prefabList.Add(ngoUIRootCharacterSelect);
        }


        public override void Start()
        {
            base.Start();
            Debug.Log("Starting RoomSceneGameObjectInstaller");
            
            if (NetworkManager.Singleton == null)
                return;

            foreach (var prefab in prefabList)
            {
                NgoZenjectHandler handler = new NgoZenjectHandler(Container,prefab);
                NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, handler);
                Debug.Log("등록");
            }
        }
    }
    
    
}
