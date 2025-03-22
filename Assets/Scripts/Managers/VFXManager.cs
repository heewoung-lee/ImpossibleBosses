using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class VFXManager
{

    GameObject _vfx_Root_NGO;

    public Transform VFX_Root_NGO
    {
        get
        {
            if (_vfx_Root_NGO == null)
            {
                Managers.RelayManager.NGO_RPC_Caller.CreatePrefabServerRpc("NGO/VFX_Root_NGO");
            }
            return _vfx_Root_NGO.transform;
        }
    }

    public void Set_VFX_Root_NGO(NetworkObject ngo)
    {
        _vfx_Root_NGO = ngo.gameObject;
    }

    private GameObject GenerateParticleInternal(string path,Vector3 pos,float settingDuration,Transform followTarget = null)
    {
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path,VFX_Root_NGO);
        particleObject.SetActive(false);
        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();

        particleObject.transform.position = pos;
        particleObject.transform.SetParent(VFX_Root_NGO);
        particleObject.SetActive(true);

        float maxDurationTime = 0f;

        if (particleObject.GetComponent<LoopingParticle>())
        {
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

    public GameObject GenerateParticle(string path, Vector3 pos = default,float settingDuration = -1f)
    {
        return GenerateParticleInternal(path,pos,settingDuration);
    }
    public GameObject GenerateParticle(string path, Transform generatorTr, float settingDuration = -1f)//쫒아가는 파티클을 위해 나눠놓음
    {
        return GenerateParticleInternal(path, generatorTr.position, settingDuration, generatorTr);
    }


    private IEnumerator FollowingGenerator(Transform generatorTr, GameObject particle)
    {
        while(particle != null)
        {
            particle.transform.position = new Vector3(generatorTr.position.x,particle.transform.position.y,generatorTr.position.z);
            yield return generatorTr;
        }
    }
}