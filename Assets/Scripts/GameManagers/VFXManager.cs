using System;
using System.Collections;
using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using NetWork.NGO;
using Unity.Netcode;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    public struct ParticleInfo
    {
        public bool IsNetworkObject;
        public bool IsLooping;
    }

    public class VFXManager
    {

        [Inject] private IInstantiate _instantiate;
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] IDestroyObject _destroyer;
        [Inject] private RelayManager _relayManager;

        
        Dictionary<string, ParticleInfo> _isCheckNgoDict = new Dictionary<string, ParticleInfo>();

        GameObject _vfxRoot;

        public Transform VFXRoot
        {
            get
            {
                if (_vfxRoot == null)
                {
                    GameObject root = new GameObject(name: "VFX_ROOT");
                    _vfxRoot = root;
                }
                return _vfxRoot.transform;
            }
        }

        GameObject _vfxRootNgo;

        public Transform VFXRootNgo
        {
            get
            {
                if(_vfxRootNgo == null)
                {
                    _vfxRootNgo = _relayManager.SpawnNetworkObj("Prefabs/NGO/VFX_Root_NGO");
                }
                return _vfxRootNgo.transform;
            }
        }


        public void Set_VFX_Root_NGO(NetworkObject ngo)
        {
            _vfxRootNgo = ngo.gameObject;
        }
        public GameObject SpawnVFXLocalOrNetwork(string path, float duration, Action rpcCallSpawnParticleEvent)
        {
            if (_isCheckNgoDict.ContainsKey(path) == false)
            {
                CashingisCheckNgoDict(path);
            }
            if (_isCheckNgoDict[path].IsNetworkObject)
            {
                rpcCallSpawnParticleEvent.Invoke();
                Debug.LogWarning("This Prefab is a NetworkObject so it won't be spawned locally");
                return null;
            }
            GameObject particleObject = _instantiate.InstantiateByPath(path);

            return particleObject;
        }

        public void GenerateParticle(string path, Transform spawnTr, float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)//쫒아가는 파티클을 위해 나눠놓음
        {
            GameObject particleObject = SpawnVFXLocalOrNetwork(path, settingDuration, FindTargetNGO_Spawn);

            if (particleObject == null)// NULL 이면 네트워크가 처리
                return;

            particleObject = SetPariclePosAndLifeCycle(particleObject, path, settingDuration, SetPositionAndChasetoTagetParticle);
            addParticleActionEvent?.Invoke(particleObject);

            void FindTargetNGO_Spawn()
            {
                ulong targetNGOID = NgoRPCCaller.Invalidobjectid;
                if (spawnTr.TryGetComponentInParents(out NetworkObject networkObj))
                {
                    targetNGOID = networkObj.NetworkObjectId;
                }
                else
                {
                    Debug.Log("targetNGOID isn't Found NGO");
                    return;
                }
                _relayManager.NgoRPCCaller.SpawnVFXPrefabServerRpc(path, settingDuration, targetNGOID);
            }
            void SetPositionAndChasetoTagetParticle(GameObject particleObj)
            {
                ParticleObjectSetPosition(particleObj, spawnTr.position, VFXRoot);
                Managers.ManagersStartCoroutine(FollowingGenerator(spawnTr, particleObj));
            }
        }

        public void GenerateParticle(string path, Vector3 spawnPos = default, float settingDuration = -1f, Action<GameObject> addParticleActionEvent = null)
        {
            GameObject particleObject = SpawnVFXLocalOrNetwork(path, settingDuration, FindNgo_Spawn);

            if (particleObject == null)// NULL 이면 네트워크가 처리
                return;

            particleObject = SetPariclePosAndLifeCycle(particleObject, path, settingDuration, SetPositionParticle);
            addParticleActionEvent?.Invoke(particleObject);


            void FindNgo_Spawn()
            {
                _relayManager.NgoRPCCaller.SpawnVFXPrefabServerRpc(path, settingDuration, spawnPos);
            }
            void SetPositionParticle(GameObject particleOBJ)
            {
                ParticleObjectSetPosition(particleOBJ, spawnPos, VFXRoot);
            }
        }

        public GameObject SetPariclePosAndLifeCycle(GameObject particleObject, string path, float settingDuration, Action<GameObject> positionAndBehaviorSetterEvent)
        {
            positionAndBehaviorSetterEvent?.Invoke(particleObject);
            if (_isCheckNgoDict.TryGetValue(path, out ParticleInfo info))
            {
                if (info.IsLooping == true)
                    return particleObject;
            }
            SettingAndRuntoParticle(particleObject, settingDuration, out float maxDurationTime);
            _destroyer.DestroyObject(particleObject, maxDurationTime);
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
            while (particle != null && particle.activeSelf == true)
            {
                particle.transform.position = new Vector3(targetTr.position.x, particle.transform.position.y, targetTr.position.z);
                yield return targetTr;
            }
        }


        private void CashingisCheckNgoDict(string path)
        {
            GameObject particleObj = _resourcesLoader.Load<GameObject>(path);
            if (particleObj.TryGetComponent(out NetworkObject ngo))
            {
                _isCheckNgoDict.Add(path, new ParticleInfo()
                {
                    IsNetworkObject = true
                });
            }
            else
            {
                _isCheckNgoDict.Add(path, new ParticleInfo()
                {
                    IsNetworkObject = false
                });
            }
            ParticleInfo particleinfo = _isCheckNgoDict[path];
            if (particleObj.TryGetComponent(out LoopingParticle loopingParticle))
            {
                particleinfo.IsLooping = true;
                _isCheckNgoDict[path] = particleinfo;
            }
            else
            {
                particleinfo.IsLooping = false;
                _isCheckNgoDict[path] = particleinfo;
            }
        }

    }
}