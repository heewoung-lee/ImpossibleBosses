using Unity.VisualScripting;
using UnityEngine;

public class MoveMarkerController : MonoBehaviour
{

    private void Start()
    {
        Managers.InputManager.playerMouseClickPositionEvent -= InstantiateMoveMarker;
        Managers.InputManager.playerMouseClickPositionEvent += InstantiateMoveMarker;
    }
    private void InstantiateMoveMarker(Vector3 MarkerPosition)
    {
        Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/WayPointEffect/Move", MarkerPosition);
    }
}