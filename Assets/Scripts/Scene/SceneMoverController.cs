using UnityEngine;

public class SceneMoverController
{
    ISceneMover _sceneMover;
    public SceneMoverController(ISceneMover sceneMover)
    {

        _sceneMover = sceneMover;
    }

    public void MoveScene()
    {
        _sceneMover.MoveScene();
    }

}
