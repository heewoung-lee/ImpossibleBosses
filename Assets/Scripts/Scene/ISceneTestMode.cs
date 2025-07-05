namespace Scene
{
    /// <summary>
    /// 인터페이스는 각 씬마다 상속해서 쓸것
    /// Base씬에 구현 하려고 했으나, 로딩씬 같은 단독씬이 있는 경우가 있어서
    /// 테스트모드가 필요한 씬들은 상속해서 쓸것
    /// </summary>
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