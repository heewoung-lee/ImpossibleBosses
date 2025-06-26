using System.Linq;
using UnityEngine;
using Zenject;

namespace Scene.ZenjectInstaller
{
   [DisallowMultipleComponent]
    public class AutoInstallBinder : MonoInstaller
    {
        public override void InstallBindings()
        {
            MonoInstaller[] installers = 
                this.GetComponents<MonoInstaller>()
                    .Where(installer => installer != this).ToArray();


            foreach (MonoInstaller installer in installers)
            {
                Container.Inject(installer);
                installer.InstallBindings();    
            }
        }
    }
}
