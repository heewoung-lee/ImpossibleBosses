using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SocketEventManager
{
    public Func<Task> DisconnectApiEvent;


    public Func<Task> DisconnectRelayEvent;
    public Func<Task> LogoutVivoxEvent;
    public Func<Task> LogoutAllLeaveLobbyEvent;



    public Action<GameObject> DonePlayerSpawnEvent;
}
