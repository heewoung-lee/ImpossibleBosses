namespace Scene
{
    public enum MultiMode
    {
        Solo,
        Multi
    }

    public interface ISceneMultiMode
    {
        public MultiMode GetMultiTestMode();
    }
}