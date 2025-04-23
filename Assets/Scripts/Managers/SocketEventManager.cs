using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class SocketEventManager
{
    private Func<Task> _disconnectRelayEvent;
    private Func<Task> _logoutVivoxEvent;
    private Func<Task> _logoutAllLeaveLobbyEvent;
    private Action<GameObject> _donePlayerSpawnEvent;

    public event Func<Task> DisconnectRelayEvent
    {
        add
        {
            if (_disconnectRelayEvent == null || !_disconnectRelayEvent.GetInvocationList().Contains(value))
                _disconnectRelayEvent += value;
        }
        remove
        {
            if (_disconnectRelayEvent == null || !_disconnectRelayEvent.GetInvocationList().Contains(value))
            {
                Debug.LogWarning($"[SocketEvent] Remove 실패: {value?.Method.Name}");
                return;
            }
            _disconnectRelayEvent -= value;
        }
    }

    public event Func<Task> LogoutVivoxEvent
    {
        add
        {
            if (_logoutVivoxEvent == null || !_logoutVivoxEvent.GetInvocationList().Contains(value))
                _logoutVivoxEvent += value;
        }
        remove
        {
            if (_logoutVivoxEvent == null || !_logoutVivoxEvent.GetInvocationList().Contains(value))
            {
                Debug.LogWarning($"[SocketEvent] Remove 실패: {value?.Method.Name}");
                return;
            }
            _logoutVivoxEvent -= value;
        }
    }

    public event Func<Task> LogoutAllLeaveLobbyEvent
    {
        add
        {
            if (_logoutAllLeaveLobbyEvent == null || !_logoutAllLeaveLobbyEvent.GetInvocationList().Contains(value))
                _logoutAllLeaveLobbyEvent += value;
        }
        remove
        {
            if (_logoutAllLeaveLobbyEvent == null || !_logoutAllLeaveLobbyEvent.GetInvocationList().Contains(value))
            {
                Debug.LogWarning($"[SocketEvent] Remove 실패: {value?.Method.Name}");
                return;
            }
            _logoutAllLeaveLobbyEvent -= value;
        }
    }

    public event Action<GameObject> DonePlayerSpawnEvent
    {
        add
        {
            if (_donePlayerSpawnEvent == null || !_donePlayerSpawnEvent.GetInvocationList().Contains(value))
                _donePlayerSpawnEvent += value;
        }
        remove
        {
            if (_donePlayerSpawnEvent == null || !_donePlayerSpawnEvent.GetInvocationList().Contains(value))
            {
                Debug.LogWarning($"[SocketEvent] Remove 실패: {value?.Method.Name}");
                return;
            }
            _donePlayerSpawnEvent -= value;
        }
    }

    public Task InvokeDisconnectRelayEvent() => _disconnectRelayEvent?.Invoke() ?? Task.CompletedTask;
    public Task InvokeLogoutVivoxEvent() => _logoutVivoxEvent?.Invoke() ?? Task.CompletedTask;
    public Task InvokeLogoutAllLeaveLobbyEvent() => _logoutAllLeaveLobbyEvent?.Invoke() ?? Task.CompletedTask;
    public void InvokeDonePlayerSpawnEvent(GameObject go) => _donePlayerSpawnEvent?.Invoke(go);
}
