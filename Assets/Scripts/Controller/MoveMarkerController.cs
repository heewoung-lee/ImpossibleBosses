using GameManagers;
using GameManagers.Interface.GameManagerEx;
using NetWork.NGO;
using Player;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class MoveMarkerController : MonoBehaviour
    {
        //TODO: 무브 마커 플레이 씬 이상에다가 바인드할것
        [Inject] IPlayerSpawnManager _gameManagerEx;
        private void RegiterPlayerMoveMarker(PlayerController controller)
        {
            controller.OnPlayerMouseClickPosition += InstantiateMoveMarker;
        }
        //현재 문제 Controller가 늦게 달림.
        
        private void UnRegiterPlayerMoveMarker(PlayerController controller)
        {
            controller.OnPlayerMouseClickPosition -= InstantiateMoveMarker;
        }
        
        private void OnEnable()
        {
            if (_gameManagerEx.GetPlayer() == null || _gameManagerEx.GetPlayer().GetComponent<PlayerController>() == null)
            {
                _gameManagerEx.GetPlayer().GetComponent<PlayerInitializeNgo>().OnPlayerSpawnwithController += RegiterPlayerMoveMarker;
            }
            else
            {
                RegiterPlayerMoveMarker(_gameManagerEx.GetPlayer().GetComponent<PlayerController>());
            }
        }

        private void OnDisable()
        {
            _gameManagerEx.GetPlayer().GetComponent<PlayerInitializeNgo>().OnPlayerSpawnwithController -= RegiterPlayerMoveMarker;
            //구독되어있는 마커이벤트를 빼주고,
            GameObject player = _gameManagerEx.GetPlayer();
            if (player != null && player.TryGetComponent(out PlayerController controller) == true)
            {
                UnRegiterPlayerMoveMarker(controller);
            }
            //해당 클래스가 없어지는데 플레이어가 남아있다면. 플레이어에게 등록된 이벤트도 같이 지워준다.
        }

        private void InstantiateMoveMarker(Vector3 markerPosition)
        {
            Managers.VFXManager.GenerateParticle("Prefabs/Paticle/WayPointEffect/Move", markerPosition);
        }
    }
}