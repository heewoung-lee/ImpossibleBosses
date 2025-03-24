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
    public GameObject TrySpawnLocalVFXOrRequestNetwork(string path, float duration)
    {
        if (_isCheckNGODict.ContainsKey(path) == false)
        {
            CashingisCheckNGODict(path);
        }
        if (_isCheckNGODict[path].isNetworkObject)
        {
            Managers.RelayManager.NGO_RPC_Caller.SpawnVFXPrefabServerRpc(path, duration);
            Debug.LogWarning("This Prefab is a NetworkObject so it won't be spawned locally");
            return null;
        }
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path);

        return particleObject;
    }

    public GameObject GenerateParticle(string path, Transform generatorTr, float settingDuration = -1f)//쫒아가는 파티클을 위해 나눠놓음
    {
        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration);
        return SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, generatorTr, settingDuration);
    }

    public GameObject GenerateParticle(string path, Vector3 generatePos = default, float settingDuration = -1f)
    {
        GameObject particleObject = TrySpawnLocalVFXOrRequestNetwork(path, settingDuration);
        return SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, generatePos, settingDuration);
    }
    //TODO:중복되는 부분 ACtion매개변수로 넘겨받아 처리할것
    public GameObject SetPariclePosAndLifeCycle(GameObject particleObject, Transform parentTr, string path, Vector3 generatePos, float settingDuration)
    {
        if (particleObject == null)
            return null;

        ParticleSystem[] particles = ParticleObjectSetPosition(particleObject, generatePos, parentTr);
        if (_isCheckNGODict.TryGetValue(path, out ParticleInfo info))
        {
            if (info.isLooping == true)
                return particleObject;
        }
        SettingAndRuntoParticle(particles, settingDuration,out float maxDurationTime);
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }
    public GameObject SetPariclePosAndLifeCycle(GameObject particleObject,Transform parentTr ,string path, Transform generateTR, float settingDuration)
    {
        if (particleObject == null)
            return null;

        ParticleSystem[] particles =  ParticleObjectSetPosition(particleObject, generateTR.position, parentTr);

        if (_isCheckNGODict.TryGetValue(path, out ParticleInfo info))
        {
            if (info.isLooping == true)
                return particleObject;
        }

        Managers.ManagersStartCoroutine(FollowingGenerator(generateTR, particleObject));
        SettingAndRuntoParticle(particles, settingDuration, out float maxDurationTime);
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }

    private void SettingAndRuntoParticle(ParticleSystem[] particles,float settingDuration,out float maxDurationTime)
    {
        maxDurationTime = 0f;
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

    private ParticleSystem[] ParticleObjectSetPosition(GameObject particleObject,Vector3 generatePos,Transform parentTr)
    {
        particleObject.SetActive(false);
        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();
        particleObject.transform.position = generatePos;
        particleObject.transform.SetParent(parentTr);
        particleObject.SetActive(true);

        return particles;
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