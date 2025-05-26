using UnityEngine;

public interface ISceneSpawnBehaviour
{
    ISceneMover nextscene { get; }

    public void SpawnOBJ();

    public void Init();

 }
