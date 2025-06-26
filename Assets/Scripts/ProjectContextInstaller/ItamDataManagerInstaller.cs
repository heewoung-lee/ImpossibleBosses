using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
   [DisallowMultipleComponent]
   public class ItamDataManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<ItemDataManager>().AsSingle();
      }
   }
}
