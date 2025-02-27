using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using UnityEngine;

public class PlaySceneTestCode : MonoBehaviour
{
    private async void Start()
    {
        await SetAuthenticationService();
        await Managers.RelayManager.StartHostWithRelay(5);

    }



    private async Task SetAuthenticationService()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}