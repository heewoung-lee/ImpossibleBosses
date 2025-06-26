using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
   [DisallowMultipleComponent]
   public class InputManagerInstaller : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<InputManager>().AsSingle();
      }
   }
}
