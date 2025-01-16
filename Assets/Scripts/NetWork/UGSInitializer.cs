using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
public class UGSInitializer : MonoBehaviour
{
    private async void Start()
    {
        // UGS √ ±‚»≠
       await UnityServices.InitializeAsync();
       await UpdateDisplayName(Managers.LogInManager.CurrentPlayerInfo.NickName);
    }

    private async System.Threading.Tasks.Task UpdateDisplayName(string newDisplayName)
    {
        try
        {
            object value = await AuthenticationService.Instance.UpdatePlayerNameAsync(newDisplayName);
            Debug.Log($"Display Name updated to: {newDisplayName}");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"Failed to update Display Name: {e.Message}");
        }
    }
}
