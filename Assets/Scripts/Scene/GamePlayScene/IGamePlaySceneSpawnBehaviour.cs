using UnityEngine;

public interface IGamePlaySceneSpawnBehaviour
{
    ISceneMover sceneMover { get; }

    public void SpawnOBJ();

    public void Init();

 }
