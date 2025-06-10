using UnityEngine;

namespace Controller
{
    public class MoveMarkerController : MonoBehaviour
    {

        private void Start()
        {
            Managers.InputManager.playerMouseClickPositionEvent -= InstantiateMoveMarker;
            Managers.InputManager.playerMouseClickPositionEvent += InstantiateMoveMarker;
        }
        private void InstantiateMoveMarker(Vector3 markerPosition)
        {
            Managers.VFX_Manager.GenerateParticle("Prefabs/Paticle/WayPointEffect/Move", markerPosition);
        }
    }
}