using GameManagers;
using UnityEngine;
using Zenject;

namespace ProjectContextInstaller
{
    [DisallowMultipleComponent]
    public class SkillManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SkillManager>().AsSingle();
        }
    }
}
