namespace Scene.ZenjectInstaller
{
    public interface IRegistrar<in T>
    {
        public void Register(T sceneContext);
        public void Unregister(T sceneContext);
    }
}
