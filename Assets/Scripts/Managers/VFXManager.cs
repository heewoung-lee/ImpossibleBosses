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

    public GameObject GenerateParticle(string path, Vector3 pos = default, float? duration = null)
    {
        GameObject particleObject = Managers.ResourceManager.InstantiatePrefab(path, VFX_Root);
        particleObject.SetActive(false);
        ParticleSystem particle = particleObject.GetComponent<ParticleSystem>();

        float defaultDuration = particle.main.duration;
        if (duration != null)
        {
            defaultDuration = duration.Value;
        }
        Managers.ManagersStartCoroutine(FadeOutOverDuration(defaultDuration, particle));

        // 위치와 부모 설정
        particle.Play();
        particleObject.transform.position = pos;
        particleObject.transform.SetParent(VFX_Root);
        particleObject.SetActive(true);
        if (particle.main.loop == false)
        {
            Managers.ResourceManager.DestroyObject(particleObject, defaultDuration);
        }
        return particleObject;
    }



    IEnumerator FadeOutOverDuration(float duration, ParticleSystem particle)
    {
        float elasedTime = 0f;

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particle.main.maxParticles];

        while (elasedTime < duration)
        {
            if (particle == null || particle.gameObject == null)
            {
                yield break; // 파괴된 경우 코루틴 종료
            }
            elasedTime += Time.deltaTime/ duration;
            float alpha = Mathf.Lerp(1,0, elasedTime);
            int aliveParticleNum = particle.GetParticles(particles);

            for (int i = 0;  i < aliveParticleNum; i++)
            {
                Color color = particles[i].startColor;
                color.a = alpha;
                particles[i].startColor = color;
            }

            particle.SetParticles(particles, aliveParticleNum);

            yield return null;
        }

    }

}