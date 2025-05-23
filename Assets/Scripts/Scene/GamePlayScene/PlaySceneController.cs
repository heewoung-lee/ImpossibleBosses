using UnityEngine;

public class PlaySceneController
{
    public PlaySceneController(IGamePlaySceneSpawnBehaviour igamePlayScene)
    {
        this.igamePlayScene = igamePlayScene;
    }

    private IGamePlaySceneSpawnBehaviour igamePlayScene;

    public void InitGamePlayScene()
    {
        igamePlayScene.Init();
    }


    public void SpawnOBJ()
    {
        igamePlayScene.SpawnOBJ();
    }
}
