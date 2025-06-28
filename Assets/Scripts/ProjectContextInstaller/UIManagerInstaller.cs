using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
   [DisallowMultipleComponent]
   public class UIManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<UIManagerCaching>().AsSingle().NonLazy();
      }
   }
}
