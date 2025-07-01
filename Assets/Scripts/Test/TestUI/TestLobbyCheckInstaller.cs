using System;
using GameManagers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Test.TestUI
{
    public class TestLobbyCheckInstaller : MonoBehaviour
    {
        [Inject] private LobbyManager _lobbyManager;

        private void Start()
        {
            GameObject checkObj = new GameObject("CheckButton");
            Button checkButton =  checkObj.AddComponent<Button>();
            
            checkButton.transform.position = new Vector3(0, 0, 0);
            
            checkButton.onClick.AddListener(async () =>
            {
                await _lobbyManager.ShowUpdatedLobbyPlayers();
            });
        }
    }
}
