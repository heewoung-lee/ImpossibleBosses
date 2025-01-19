using System;
using System.Threading.Tasks;

public class SocketEventManager
{
    public Func<Task> OnApplicationQuitEvent;
    public Func<Task> DisconnectApiEvent;
}