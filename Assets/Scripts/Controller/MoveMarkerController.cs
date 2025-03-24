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
        GameObject paticle = Managers.VFX_Manager.GenerateLocalParticle("Paticle/WayPointEffect/Move",MarkerPosition);
    }
}