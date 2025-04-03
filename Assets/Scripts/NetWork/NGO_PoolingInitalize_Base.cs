public abstract class NGO_PoolingInitalize_Base : NGO_Skill_Initailize_Base
{
    public abstract string PoolingNGO_PATH { get; }
    public virtual void OnPoolGet() { }
    public virtual void OnPoolRelease() { }
    public abstract int PoolingCapacity { get; }
}