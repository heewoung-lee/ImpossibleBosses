using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private GameObject GenerateParticleInternal(string path, Vector3 pos, float settingDuration, Transform followTarget = null)
    {

        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path);

        CashingisCheckNGODict(particleObject, path);
        Transform parentTr = _isCheckNGODict[path].isNetworkObject == true ? VFX_Root_NGO : VFX_Root;

        if (_isCheckNGODict[path].isNetworkObject)
        {
            Managers.RelayManager.NGO_RPC_Caller.CreatePrefabServerRpc(path);
        }


        particleObject.SetActive(false);
        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();
        particleObject.transform.position = pos;
        particleObject.transform.SetParent(parentTr);
        particleObject.SetActive(true);

        float maxDurationTime = 0f;

        if (_isCheckNGODict.TryGetValue(path,out ParticleInfo info))
        {
            if(info.isLooping == true)
            return particleObject;
        }

        if (followTarget != null)
        {
            Managers.ManagersStartCoroutine(FollowingGenerator(followTarget, particleObject));
        }
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
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }

    public GameObject GenerateParticle(string path, Vector3 pos = default, float settingDuration = -1f)
    {
        return GenerateParticleInternal(path, pos, settingDuration);
    }
    public GameObject GenerateParticle(string path, Transform generatorTr, float settingDuration = -1f)//쫒아가는 파티클을 위해 나눠놓음
    {
        return GenerateParticleInternal(path, generatorTr.position, settingDuration, generatorTr);
    }


    private IEnumerator FollowingGenerator(Transform generatorTr, GameObject particle)
    {
        while (particle != null)
        {
            particle.transform.position = new Vector3(generatorTr.position.x, particle.transform.position.y, generatorTr.position.z);
            yield return generatorTr;
        }
    }


    private void CashingisCheckNGODict(GameObject particleObject,string path)
    {
        if (_isCheckNGODict.ContainsKey(path) == false)
        {
            if (particleObject.TryGetComponent(out NetworkObject ngo))
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
            if (particleObject.TryGetComponent(out LoopingParticle loopingParticle))
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
}