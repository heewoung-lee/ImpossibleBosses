using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;

public struct ParticleInfo
{
    public bool isNetworkObject;
    public bool isLooping;
}

public class VFXManager
{

    Dictionary<string, ParticleInfo> _isCheckNGODict = new Dictionary<string, ParticleInfo>();

    GameObject _vfx_Root;

    public Transform VFX_Root
    {
        get
        {
            if (_vfx_Root == null)
            {
                GameObject root = new GameObject(name: "VFX_ROOT");
                _vfx_Root = root;
            }
            return _vfx_Root.transform;
        }
    }

    GameObject _vfx_Root_NGO;

    public Transform VFX_Root_NGO { get => _vfx_Root_NGO.transform; }
    public void Set_VFX_Root_NGO(NetworkObject ngo)
    {
        _vfx_Root_NGO = ngo.gameObject;
    }
    public GameObject TrySpawnLocalVFXOrRequestNetwork(string path, float duration,Action rpcCallSpawnParticleEvent)
    {
        if (_isCheckNGODict.ContainsKey(path) == false)
        {
            CashingisCheckNGODict(path);
        }
        if (_isCheckNGODict[path].isNetworkObject)
        {
            rpcCallSpawnParticleEvent.Invoke();
            Debug.LogWarning("This Prefab is a NetworkObject so it won't be spawned locally");
            return null;
        }
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path);

        return particleObject;
    }

    public void GenerateParticle(string path, Transform spawnTr,float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)//�i�ư��� ��ƼŬ�� ���� ��������
    {
        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration, () =>
        {
            ulong targetNGOID = NGO_RPC_Caller.INVALIDOBJECTID;
            if (spawnTr.TryGetComponent(out NetworkObject networkObj))
            {
                targetNGOID = networkObj.NetworkObjectId;
            }
            Managers.RelayManager.NGO_RPC_Caller.SpawnVFXPrefabServerRpc(path, settingDuration, targetNGOID);
        });

        if (particleObject == null)// NULL �̸� ��Ʈ��ũ�� ó��
            return;

        particleObject = SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, settingDuration, (paritcleOBJ) =>
        {
            ParticleObjectSetPosition(paritcleOBJ,spawnTr.position, VFX_Root);
            Managers.ManagersStartCoroutine(FollowingGenerator(spawnTr, particleObject));
        });
        addParticleActionEvent?.Invoke(particleObject);
    }

    public void GenerateParticle(string path, Vector3 spawnPos = default, float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)
    {
        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration, () =>
        {
            Managers.RelayManager.NGO_RPC_Caller.SpawnVFXPrefabServerRpc(path, settingDuration, spawnPos);
        });

        if (particleObject == null)// NULL �̸� ��Ʈ��ũ�� ó��
            return;

        particleObject = SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, settingDuration, (paritcleOBJ) =>
        {
            ParticleObjectSetPosition(paritcleOBJ, spawnPos, VFX_Root);
        });
        addParticleActionEvent?.Invoke(particleObject);
    }

    public GameObject SetPariclePosAndLifeCycle(GameObject particleObject, Transform parentTr, string path, float settingDuration, Action<GameObject> positionAndBehaviorSetterEvent)
    {

        positionAndBehaviorSetterEvent?.Invoke(particleObject);
        if (_isCheckNGODict.TryGetValue(path, out ParticleInfo info))
        {
            if (info.isLooping == true)
                return particleObject;
        }
        SettingAndRuntoParticle(particleObject, settingDuration, out float maxDurationTime);
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }

    private void SettingAndRuntoParticle(GameObject particleObject, float settingDuration,out float maxDurationTime)
    {
        maxDurationTime = 0f;

        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
            particle.Clear();
            float duration = 0f;
            ParticleSystem.MainModule main = particle.main;

            duration = settingDuration <= 0 ? main.duration : settingDuration;
            main.duration = duration;
            if (particle.GetComponent<ParticleLifetimeSync>())//��ƼŬ �ý����� Duration�� �ð��� ����� �ϴ� ��ƼŬ�� �ִٸ� ����
            {
                main.startLifetime = duration;
            }
            else if (duration < particle.main.startLifetime.constantMax)//Duration���� ��ƼŬ �����ð��� ū ��� ��ƼŬ �����ð��� �ִ´�.
            {
                maxDurationTime = particle.main.startLifetime.constantMax;
            }
            else if (maxDurationTime < duration + particle.main.startLifetime.constantMax && particle.GetComponent<ParticleLifetimeSync>() == null)
            {
                maxDurationTime = duration + particle.main.startLifetime.constantMax;
            }
            particle.Play();
        }
    }

    private void ParticleObjectSetPosition(GameObject particleObject,Vector3 generatePos,Transform parentTr)
    {
        particleObject.SetActive(false);
        particleObject.transform.position = generatePos;
        particleObject.transform.SetParent(parentTr);
        particleObject.SetActive(true);
    }

    private IEnumerator FollowingGenerator(Transform generatorTr, GameObject particle)
    {
        while (particle != null)
        {
            particle.transform.position = new Vector3(generatorTr.position.x, particle.transform.position.y, generatorTr.position.z);
            yield return generatorTr;
        }
    }


    private void CashingisCheckNGODict(string path)
    {
        GameObject particleOBJ = Managers.ResourceManager.Load<GameObject>("Prefabs/" + path);
        if (particleOBJ.TryGetComponent(out NetworkObject ngo))
        {
            _isCheckNGODict.Add(path, new ParticleInfo()
            {
                isNetworkObject = true
            });
        }
        else
        {
            _isCheckNGODict.Add(path, new ParticleInfo()
            {
                isNetworkObject = false
            });
        }
        ParticleInfo particleinfo = _isCheckNGODict[path];
        if (particleOBJ.TryGetComponent(out LoopingParticle loopingParticle))
        {
            particleinfo.isLooping = true;
            _isCheckNGODict[path] = particleinfo;
        }
        else
        {
            particleinfo.isLooping = false;
            _isCheckNGODict[path] = particleinfo;
        }
    }
}