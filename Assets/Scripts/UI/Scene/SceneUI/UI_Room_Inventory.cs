using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Threading.Tasks;

public class UI_Room_Inventory : UI_Scene
{    enum Transforms
    {
        Room_Content
    }

    private Transform _room_Content;
    public Transform Room_Content => _room_Content;
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
}
