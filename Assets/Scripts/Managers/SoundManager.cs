using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class SoundManager : IManagerInitializable,IManagerIResettable
{
    AudioSource[] audioSources = new AudioSource[System.Enum.GetValues(typeof(Define.Sound)).Length];

    Dictionary<string,AudioClip> _sfxDictionnary = new Dictionary<string,AudioClip>();


    //Init()���� ���� ������ @Sound �Ŵ����� �ִ��� Ȯ��.
    //���ٸ� ���� ����� �ٸ��������� �� �μ����Բ� ����

    //������ҽ� �迭�� ����� �ϳ��� BGM������
    //������ �ϳ��� ����Ʈ������ ����

    //�����Ÿ�Կ� ���� �̸��� �����µ�
    //@Sound��ü�� �ڽ�����
    //BGM�� �̸��� ���ӿ�����Ʈ��
    //����Ʈ�� ���ӿ�����Ʈ�� ���� �������

    //BGM�� ����� �ݺ�������� ����


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
            audioSources[i] = Utill.GetOrAddComponent<AudioSource>(sound);
            sound.transform.parent = go.transform;
        }

        audioSources[(int)Define.Sound.BGM].loop = true;
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

        AudioSource source = audioSources[(int)type];

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
        foreach(AudioSource audiosource in audioSources)
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
            clip = Managers.ResourceManager.Load<AudioClip>(path);
        }
        else
        {
            clip = null;
            if(_sfxDictionnary.TryGetValue(path,out clip) == false)
            {
                clip = Managers.ResourceManager.Load<AudioClip>(path);
                _sfxDictionnary.Add(path, clip);
            }
        }

        if (clip == null)
            Debug.Log("Fail to Load Clip");


        return clip;
    }


}
