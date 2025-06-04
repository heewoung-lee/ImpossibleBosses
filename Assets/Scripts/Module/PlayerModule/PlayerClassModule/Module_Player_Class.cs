using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Module_Player_Class : MonoBehaviour
{
    public abstract Define.PlayerClass PlayerClass { get; }


    private Dictionary<string, BaseSkill> _playerSkill;

    public virtual void InitializeOnAwake()
    {
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted += ChangeLoadScene;
    }

    public virtual void InitializeOnStart()
    {
        //    if(Managers.SceneManagerEx.GetCurrentScene is ISkillInit)
        //    {
        //        InitializeSkillsFromManager();
        //    }
    }

    public void OnDestroy()
    {
        Managers.RelayManager.NetworkManagerEx.SceneManager.OnLoadEventCompleted -= ChangeLoadScene;
    }

    private void ChangeLoadScene(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {

        if (Managers.SceneManagerEx.GetCurrentScene is not ISkillInit)
            return;

        if (clientsCompleted.Contains(Managers.RelayManager.NetworkManagerEx.LocalClientId) is false)
            return;

        Debug.Log("이건 씬변경시 호출");
        InitializeSkillsFromManager();
    }

    private void InitializeSkillsFromManager()
    {
        if (GetComponent<NetworkObject>().IsOwner == false)
            return;

        _playerSkill = Managers.SkillManager.AllSKillDict
            .Where(skill => skill.Value.PlayerClass == PlayerClass)
            .ToDictionary(skill => skill.Key, skill => skill.Value);//각 클래스에 맞는 스킬들을 추린다

        if (Managers.SkillManager.UI_SkillBar == null)
        {
            Managers.SkillManager.Done_UI_SKilBar_Init_Event += AssignSkillsToUISlots;
        }
        else
        {
            AssignSkillsToUISlots();
        }
    }


    public void AssignSkillsToUISlots()
    {
        foreach (BaseSkill skill in _playerSkill.Values)
        {
            GameObject skillPrefab = Managers.ResourceManager.Instantiate("Prefabs/UI/Skill/UI_SkillComponent");
            SkillComponent skillcomponent = skillPrefab.GetOrAddComponent<SkillComponent>();
            skillcomponent.SetSkillComponent(skill);
            Transform skillLocation = Managers.SkillManager.UI_SkillBar.SetLocationSkillSlot(skillcomponent);
            skillcomponent.AttachItemToSlot(skillcomponent.gameObject, skillLocation);
        }
    }
    private void Awake()
    {
        _playerSkill = new Dictionary<string, BaseSkill>();
        InitializeOnAwake();
    }

    private void Start()
    {
        InitializeOnStart();
    }
}
