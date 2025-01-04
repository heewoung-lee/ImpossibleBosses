using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class VFXManager
{

    GameObject _vfx_Root;

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

    public GameObject GenerateParticle(string path, Vector3 pos = default,float duration = -1f)
    {
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path, VFX_Root);
        particleObject.SetActive(false);
        ParticleSystem[] particles = particleObject.GetComponentsInChildren<ParticleSystem>();

        particleObject.transform.position = pos;
        particleObject.transform.SetParent(VFX_Root);
        particleObject.SetActive(true);

        float maxDurationTime = 0f;
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
            float defaultDuration = 0f;
            ParticleSystem.MainModule main = particle.main;
            if (duration <= 0f)
            {
                defaultDuration = main.duration;
            }
            else
            {
                main.duration = duration;
                defaultDuration = duration;

                if (particle.GetComponent<ParticleLifetimeSync>())//��ƼŬ �ý����� Duration�� �ð��� ����� �ϴ� ��ƼŬ�� �ִٸ� ����
                {
                    main.startLifetime = duration;
                }
            }
            if (maxDurationTime < defaultDuration + particle.main.startLifetime.constantMax)
            {
                maxDurationTime = defaultDuration + particle.main.startLifetime.constantMax;
            }
            // ��ġ�� �θ� ����
            particle.Play();
        }
        Managers.ResourceManager.DestroyObject(particleObject, maxDurationTime);
        return particleObject;
    }
}