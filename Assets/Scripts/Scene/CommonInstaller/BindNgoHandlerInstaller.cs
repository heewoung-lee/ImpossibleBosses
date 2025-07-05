using System.Collections.Generic;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scene.CommonInstaller
{
    public class BindNgoHandlerInstaller : MonoInstaller
    {

        public void InputBindNgoHandler(List<GameObject> networkObjects)
        {
            BindNgoHandler(networkObjects);
        }
        
        private void BindNgoHandler(List<GameObject> networkObjects)
        {
            if (NetworkManager.Singleton == null)
                return;
           
            NgoZenjectHandler.NgoZenjectHandlerFactory factory = Container.Resolve<NgoZenjectHandler.NgoZenjectHandlerFactory>();

            foreach (GameObject networkObject in networkObjects)
            {
                NgoZenjectHandler handler = factory.Create(networkObject);
                NetworkManager.Singleton.PrefabHandler.AddHandler(networkObject, handler);
            }
        }
    }
}
