using NUnit.Framework.Constraints;
using System;
using System.IO;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NGO_Pool_RootInitailize : NetworkBehaviour
{
    NetworkVariable<FixedString64Bytes> _rootName = new NetworkVariable<FixedString64Bytes>
    ("",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    NetworkVariable<FixedString128Bytes> _poolingNgoPath = new NetworkVariable<FixedString128Bytes>
   ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _rootName.OnValueChanged -= OnChangedRootname;
        _rootName.OnValueChanged += OnChangedRootname;

        if (IsHost == false)//Ŭ���̾�Ʈ�� �ٲ�ٴ� �ݹ��� ������ �� ������ �������� Ȯ��
        {
            string objectName = gameObject.name;
            string newName = _rootName.Value.ToString();

            //�� ���� �ٸ��� -> �� �̹� ������ _rootName�� �����ص� ��Ȳ
            if (objectName != newName && string.IsNullOrEmpty(newName) == false)
            {
                gameObject.name = newName;
                GeneratePoolOBJ();
            }
        }
    }

    private void OnChangedRootname(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        gameObject.name = newValue.ToString();
        GeneratePoolOBJ();
    }

    private void GeneratePoolOBJ()
    {
        GameObject ngo = Managers.ResourceManager.Load<GameObject>(_poolingNgoPath.Value.ToString());
        if (ngo.TryGetComponent(out NGO_PoolingInitalize_Base poolingOBJ))
        {
            Managers.NGO_PoolManager.SetPool_NGO_ROOT_Dict(poolingOBJ.PoolingNGO_PATH, transform);
            Managers.NGO_PoolManager.NGO_Pool_RegisterPrefab(poolingOBJ.PoolingNGO_PATH, poolingOBJ.PoolingCapacity);
            //��ųʸ��� �� Ǯ���� �ݳ���� ���
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
