using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class SocketEventManager
{
    private Func<Task> _disconnectApiEvent;
    private Func<Task> _disconnectRelayEvent;
    private Func<Task> _logoutVivoxEvent;
    private Func<Task> _logoutAllLeaveLobbyEvent;

    private Action<GameObject> _donePlayerSpawnEvent;

    public event Func<Task> DisconnectApiEvent
    {
        add
        {
            if (_disconnectApiEvent == null || !_disconnectApiEvent.GetInvocationList().Contains(value))
                _disconnectApiEvent += value;
        }
        remove
        {
            if (_disconnectApiEvent == null || !_disconnectApiEvent.GetInvocationList().Contains(value))
            {
                Debug.LogWarning($"[SocketEvent] Remove 실패: {value?.Method.Name}");
                return;
            }
            _disconnectApiEvent -= value;
        }
    }

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

    public Task InvokeDisconnectApiEvent() => _disconnectApiEvent?.Invoke() ?? Task.CompletedTask;
    //호출 할께 없으면 태스크를 끝난상태로 되돌려야함
    public Task InvokeDisconnectRelayEvent() => _disconnectRelayEvent?.Invoke() ?? Task.CompletedTask;
    public Task InvokeLogoutVivoxEvent() => _logoutVivoxEvent?.Invoke() ?? Task.CompletedTask;
    public Task InvokeLogoutAllLeaveLobbyEvent() => _logoutAllLeaveLobbyEvent?.Invoke() ?? Task.CompletedTask;
    public void InvokeDonePlayerSpawnEvent(GameObject go) => _donePlayerSpawnEvent?.Invoke(go);
}
