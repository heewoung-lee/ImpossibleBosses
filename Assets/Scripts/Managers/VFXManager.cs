using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
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
    public GameObject TrySpawnLocalVFXOrRequestNetwork(string path, float duration, Action rpcCallSpawnParticleEvent)
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

    public void GenerateParticle(string path, Transform spawnTr, float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)//쫒아가는 파티클을 위해 나눠놓음
    {
        void FindTargetNGO_Spawn()
        {
            ulong targetNGOID = NGO_RPC_Caller.INVALIDOBJECTID;
            if (spawnTr.TryGetComponentInParents(out NetworkObject networkObj))
            {
                targetNGOID = networkObj.NetworkObjectId;
            }
            else
            {
                Debug.Log("targetNGOID isn't Found NGO");
                return;
            }
            Managers.RelayManager.NGO_RPC_Caller.SpawnVFXPrefabServerRpc(path, settingDuration, targetNGOID);
        }

        void SetPositionAndChasetoTagetParticle(GameObject particleOBJ)
        {
            ParticleObjectSetPosition(particleOBJ, spawnTr.position, VFX_Root);
            Managers.ManagersStartCoroutine(FollowingGenerator(spawnTr, particleOBJ));
        }


        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration, FindTargetNGO_Spawn);

        if (particleObject == null)// NULL 이면 네트워크가 처리
            return;

        particleObject = SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, settingDuration, SetPositionAndChasetoTagetParticle);
        addParticleActionEvent?.Invoke(particleObject);
    }

    public void GenerateParticle(string path, Vector3 spawnPos = default, float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)
    {
        void FindNgo_Spawn()
        {
            Managers.RelayManager.NGO_RPC_Caller.SpawnVFXPrefabServerRpc(path, settingDuration, spawnPos);
        }
        void SetPositionParticle(GameObject particleOBJ)
        {
            ParticleObjectSetPosition(particleOBJ, spawnPos, VFX_Root);
        }

        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration, FindNgo_Spawn);

        if (particleObject == null)// NULL 이면 네트워크가 처리
            return;

        particleObject = SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, settingDuration, SetPositionParticle);
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

    private void SettingAndRuntoParticle(GameObject particleObject, float settingDuration, out float maxDurationTime)
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
            if (particle.GetComponent<ParticleLifetimeSync>())//파티클 시스템중 Duration과 시간을 맞춰야 하는 파티클이 있다면 적용
            {
                main.startLifetime = duration;
            }
            else if (duration < particle.main.startLifetime.constantMax)//Duration보다 파티클 생존시간이 큰 경우 파티클 생존시간을 넣는다.
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

    public void ParticleObjectSetPosition(GameObject particleObject, Vector3 generatePos, Transform parentTr)
    {
        particleObject.SetActive(false);
        particleObject.transform.position = generatePos;
        particleObject.transform.SetParent(parentTr);
        particleObject.SetActive(true);
    }

    public IEnumerator FollowingGenerator(Transform targetTr, GameObject particle)
    {
        while (particle != null)
        {
            particle.transform.position = new Vector3(targetTr.position.x, particle.transform.position.y, targetTr.position.z);
            yield return targetTr;
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