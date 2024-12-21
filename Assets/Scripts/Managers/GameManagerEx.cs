using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerEx : IManagerInitializable
{
    private GameObject _player;
    private GameObject _bossMonster;
    private Environment _environment;
    private GameObject _spawnPoint;
    private HashSet<GameObject> _enemy = new HashSet<GameObject>();

    public Action<int> SpawnEvent;
    public GameObject Player { get => _player; }
    public GameObject BossMonster { get => _bossMonster; }
    public Environment EnvironMent { get => _environment; }
    public GameObject SpawnPoint { get => _spawnPoint; }

    public HashSet<GameObject> Enemy { get => _enemy; }

    public GameObject Spawn(string path, Transform parent = null)
    {
        GameObject go = Managers.ResourceManager.Instantiate(path, parent);

        switch (GetWorldObjectType(go))
        {
            case Define.WorldObject.Unknown:
                Debug.Log($"Unkown this object: {go.name}");
                break;
            case Define.WorldObject.Player:
                _player = go;
                break;
            case Define.WorldObject.Monster:
                _enemy.Add(go);
                SpawnEvent?.Invoke(1);
                break;
            case Define.WorldObject.Boss:
                _bossMonster = go;
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        if (go.TryGetComponent(out BaseController baseController))
            return baseController.WorldobjectType;

        return Define.WorldObject.Unknown;
    }

    public void Despawn(GameObject go)
    {
        switch (GetWorldObjectType(go))
        {
            case Define.WorldObject.Unknown:
                Debug.Log($"Unkown this object: {go.name}");
                break;
            case Define.WorldObject.Player:
                _player = null;
                break;
            case Define.WorldObject.Monster:
                if (_enemy.Contains(go))
                {
                    _enemy.Remove(go);
                    SpawnEvent?.Invoke(-1);
                }
                break;
        }
        Managers.ResourceManager.DestroyObject(go);
    }

    public void Init()
    {
        _spawnPoint = new GameObject() { name = "@SpawnPoint" };
        _environment = GameObject.FindAnyObjectByType<Environment>();

        if (_environment == null)
        {
            GameObject environmentGO = new GameObject() { name = "@Environment" };
            _environment = environmentGO.GetOrAddComponent<Environment>();
        }
        foreach (Transform childTr in _environment.transform)
        {
            if (childTr.gameObject.TryGetComponentInsChildren(out SpawnPoint[] spawnPointParent))
            {
                foreach (SpawnPoint spawnPoint in spawnPointParent)
                {
                    spawnPoint.transform.SetParent(_spawnPoint.transform);
                }
            }
        }
    }
}