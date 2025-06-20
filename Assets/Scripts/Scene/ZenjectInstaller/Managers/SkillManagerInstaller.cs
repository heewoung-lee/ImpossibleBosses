using GameManagers;
using Zenject;

namespace Scene.ZenjectInstaller.Managers
{
    public class SkillManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SkillManager>().AsSingle();
        }
    }
}
