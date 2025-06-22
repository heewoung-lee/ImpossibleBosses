using System;
using System.Collections.Generic;
using GameManagers.Interface.Resources_Interface;
using UnityEngine;
using Util;
using Zenject;

namespace GameManagers
{
    public class SoundManager : IManagerInitializable,IManagerIResettable
    {
        AudioSource[] _audioSources = new AudioSource[System.Enum.GetValues(typeof(Define.Sound)).Length];

        Dictionary<string,AudioClip> _sfxDictionnary = new Dictionary<string,AudioClip>();
        [Inject] IResourcesLoader _resourcesLoader;
        [Inject] IInstantiate _instantiate;

        //Init()으로 현재 씬에서 @Sound 매니저가 있는지 확인.
        //없다면 새로 만들고 다른씬에서도 안 부셔지게끔 설정

        //오디오소스 배열을 만들어 하나는 BGM용으로
        //나머지 하나는 이펙트용으로 만듬

        //오디오타입에 대한 이름을 가져온뒤
        //@Sound객체에 자식으로
        //BGM용 이름의 게임오브젝트와
        //이펙트용 게임오브젝트를 각각 만들어줌

        //BGM용 사운드는 반복재생으로 설정


        public void Init()
        {

            GameObject go = GameObject.Find("@Sound");
            if(go == null)
            {
                go = new GameObject() { name = "@Sound" };
            }
            UnityEngine.Object.DontDestroyOnLoad(go);
            string[] _soundsType = Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i< _soundsType.Length; i++)
            {
                GameObject sound = new GameObject() { name = _soundsType[i]};
                _audioSources[i] = _instantiate.GetOrAddComponent<AudioSource>(sound);
                sound.transform.parent = go.transform;
            }

            _audioSources[(int)Define.Sound.BGM].loop = true;
        }



        public void Play(string path, Define.Sound type = Define.Sound.SFX, float pitch = 1.0f)
        {
            AudioClip clip = GetorAddClip(path, type);
            Play(clip,type, pitch);
        }
        public void Play(AudioClip clip, Define.Sound type = Define.Sound.SFX, float pitch = 1.0f)
        {

            if (clip == null)
                return;

            AudioSource source = _audioSources[(int)type];

            if (type == Define.Sound.BGM)
            {

                if(source.isPlaying)
                    source.Stop();

                source.clip = clip;
                source.pitch = pitch;
                source.Play();
            
            }
            else
            {
                source.pitch = pitch;
                source.PlayOneShot(clip);
            }
        }


        public void Clear()
        {
            foreach(AudioSource audiosource in _audioSources)
            {
                audiosource.Stop();
                audiosource.clip = null;
            }
            _sfxDictionnary.Clear();
        }

        public AudioClip GetorAddClip(string path, Define.Sound type = Define.Sound.SFX)
        {
            if (path.Contains("Sounds/") == false)
                path = $"Sounds/{path}";


            AudioClip clip = null;
            if(type == Define.Sound.BGM)
            {
                clip = _resourcesLoader.Load<AudioClip>(path);
            }
            else
            {
                clip = null;
                if(_sfxDictionnary.TryGetValue(path,out clip) == false)
                {
                    clip = _resourcesLoader.Load<AudioClip>(path);
                    _sfxDictionnary.Add(path, clip);
                }
            }

            if (clip == null)
                Debug.Log("Fail to Load Clip");


            return clip;
        }


    }
}
