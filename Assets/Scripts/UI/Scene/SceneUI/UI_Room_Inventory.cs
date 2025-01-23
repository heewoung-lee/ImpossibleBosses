using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;

public class UI_Room_Inventory : UI_Scene
{
    private bool isRefreshing =false;
    private 
    enum Transforms
    {
        Room_Content
    }

    private Transform _room_Content;
    protected override void AwakeInit()
    {
        base.AwakeInit();
        Bind<Transform>(typeof(Transforms));
        _room_Content = Get<Transform>((int)Transforms.Room_Content);

    }

    protected override void StartInit()
    {
        base.StartInit();
    }


    public async Task ReFreshRoomList()
    {
        if (isRefreshing) { return; }

        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"),
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0")
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);

            foreach (Transform child in _room_Content)
            {
                Managers.ResourceManager.DestroyObject(child.gameObject); 
            }
            foreach (Lobby lobby in lobbies.Results)
            {
                //Managers.ResourceManager.Instantiate(); TODO: 여기에 UI 생성
                //여기에 룸정보 입력하기.방제 인원 등
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            isRefreshing = false;
            throw;
        }

        isRefreshing = false;
    }
}
