using System;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using GameManagers.Interface.UIManager;
using Unity.VisualScripting;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class CachingObjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CachingObjectDictManager>().AsSingle().NonLazy();//호출부가 없으므로 캐싱 데이터를 사용하려면 논레이지로 강제 호출 해줘야 한다.
            
            Container.BindInterfacesAndSelfTo<UICachingService>().AsSingle().NonLazy();
        }
    }   
}
