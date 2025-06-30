namespace Scene
{
    public enum TestMode
    {
        Local,
        Multi
    }
    public interface ISceneTestMode
    {
        public TestMode GetTestMode();
    }
}