using System;
using System.Threading.Tasks;
using UnityEngine;

public class SocketEventManager
{
    public Func<Task> OnApplicationQuitEvent;
    public Func<Task> DisconnectApiEvent;
    public Action<GameObject> PlayerSpawnInitalize;
}