using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SocketEventManager
{
    public Func<Task> OnApplicationQuitEvent;
    public Func<Task> DisconnectApiEvent;
    public Action<GameObject> PlayerSpawnInitalize;
}
