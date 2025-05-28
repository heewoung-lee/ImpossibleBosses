using UnityEngine.AI;

public class PlayerPositionController
{
    public PlayerPositionController(IPlayerPositionController controller)
    {
        _iPlayerPositionController = controller;
    }
    private IPlayerPositionController _iPlayerPositionController;

    public void SetPlayerPosition()
    {
        _iPlayerPositionController.SetPlayerPosition();
    }

}