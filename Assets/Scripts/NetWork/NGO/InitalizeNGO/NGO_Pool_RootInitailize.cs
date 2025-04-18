using BehaviorDesigner.Runtime.Tasks.Unity.UnityString;
using NUnit.Framework.Constraints;
using System;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

public class NGO_Pool_RootInitailize : NetworkBehaviour
{
    NetworkVariable<FixedString64Bytes> _rootName = new NetworkVariable<FixedString64Bytes>
    ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    NetworkVariable<FixedString128Bytes> _poolingNgoPath = new NetworkVariable<FixedString128Bytes>
    ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsHost)
        {
            transform.SetParent(Managers.NGO_PoolManager.NGOPool.transform);
        }

        _rootName.OnValueChanged -= OnChangedRootname;
        _rootName.OnValueChanged += OnChangedRootname;

        _poolingNgoPath.OnValueChanged -= OnChangedPoolingNgoPath;
        _poolingNgoPath.OnValueChanged += OnChangedPoolingNgoPath;

        if (IsHost == false)//클라이언트는 바꿨다는 콜백을 못받을 수 있으니 수동으로 확인
        {
            string objectName = gameObject.name;
            string newName = _rootName.Value.ToString();

            //두 값이 다르면 -> 즉 이미 서버가 _rootName을 설정해둔 상황
            if (objectName != newName && string.IsNullOrEmpty(newName) == false)
            {
                gameObject.name = newName;
                GeneratePoolOBJ(_poolingNgoPath.Value.ToString());
            }
        }
    }

    private void OnChangedPoolingNgoPath(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        GeneratePoolOBJ(newValue.ToString());
    }

    private void OnChangedRootname(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        gameObject.name = newValue.ToString();
    }

    private void GeneratePoolOBJ(string path)
    {
        GameObject ngo = Managers.ResourceManager.Load<GameObject>(path);

        //_poolingNgoPath가 안뜸
        if (ngo.TryGetComponent(out NGO_PoolingInitalize_Base poolingOBJ))
        {
            Managers.NGO_PoolManager.SetPool_NGO_ROOT_Dict(poolingOBJ.PoolingNGO_PATH, transform);
            Managers.NGO_PoolManager.NGO_Pool_RegisterPrefab(poolingOBJ.PoolingNGO_PATH, poolingOBJ.PoolingCapacity);
            //딕셔너리에 각 풀마다 반납장소 등록
        }
    }

    public void SetRootObjectName(string poolingNgoPath)
    {
        _poolingNgoPath.Value = poolingNgoPath;
        string pathName = Path.GetFileNameWithoutExtension(poolingNgoPath);
        pathName += "_Root";
        _rootName.Value = pathName;
    }
}
