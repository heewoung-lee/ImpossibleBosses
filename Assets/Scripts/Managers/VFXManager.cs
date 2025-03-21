using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.ParticleSystem;

public class VFXManager
{

    GameObject _vfx_Root;
    GameObject _vfx_Root_NGO;
    public Transform VFX_Root
    {
        get
        {
            if (_vfx_Root == null)
            {
                _vfx_Root = new GameObject() { name = "VFX_ROOT" };
            }
            return _vfx_Root.transform;
        }
    }
    //TODO: vfx_ROot_Ngo ������Ƽ �����

    private GameObject GenerateParticleInternal(string path,Vector3 pos,float settingDuration,Transform followTarget = null)
    {
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path, VFX_Root);
        particleObject.SetActive(false);
        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();

        particleObject.transform.position = pos;
        particleObject.transform.SetParent(VFX_Root);
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
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }

    public GameObject GenerateParticle(string path, Vector3 pos = default,float settingDuration = -1f)
    {
        return GenerateParticleInternal(path,pos,settingDuration);
    }
    public GameObject GenerateParticle(string path, Transform generatorTr, float settingDuration = -1f)//�i�ư��� ��ƼŬ�� ���� ��������
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