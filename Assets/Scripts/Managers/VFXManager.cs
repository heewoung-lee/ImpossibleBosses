using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private GameObject GenerateParticleInternal_Base(string path, Func<GameObject, GameObject> handleSpawn)
    {
        if (_isCheckNGODict.ContainsKey(path) == false)
        {
            CashingisCheckNGODict(path);
        }

        if (_isCheckNGODict[path].isNetworkObject)
        {
            Managers.RelayManager.NGO_RPC_Caller.SpawnPrefabNeedToInitalizeRpc(path);
            return null;
        }

        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path);
        return handleSpawn.Invoke(particleObject);
    }
    public GameObject GenerateLocalParticle(string path, Transform generatorTr, float settingDuration = -1f, bool isFollowing = true)//�i�ư��� ��ƼŬ�� ���� ��������
    {
        return GenerateParticleInternal_Base(path, (particleObject) =>
      SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, generatorTr, settingDuration, isFollowing));
    }
    public GameObject GenerateLocalParticle(string path, Vector3 generatePos = default, float settingDuration = -1f)
    {
        return GenerateParticleInternal_Base(path, (particleObject) =>
        SetPariclePosAndLifeCycle(particleObject, VFX_Root, path, generatePos, settingDuration));
    }

    public GameObject SetPariclePosAndLifeCycle(GameObject particleObject, Transform parentTr, string path, Vector3 generatePos, float settingDuration)
    {
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
    public GameObject SetPariclePosAndLifeCycle(GameObject particleObject,Transform parentTr ,string path, Transform generateTR, float settingDuration, bool isfollowing = false)
    {
        ParticleSystem[] particles =  ParticleObjectSetPosition(particleObject, generateTR.position, parentTr);

        if (_isCheckNGODict.TryGetValue(path, out ParticleInfo info))
        {
            if (info.isLooping == true)
                return particleObject;
        }

        if (isfollowing == true)
        {
            Managers.ManagersStartCoroutine(FollowingGenerator(generateTR, particleObject));
        }

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