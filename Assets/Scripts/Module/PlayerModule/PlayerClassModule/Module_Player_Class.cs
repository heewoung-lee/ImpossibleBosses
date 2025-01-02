using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Module_Player_Class : MonoBehaviour
{
    public abstract Define.PlayerClass PlayerClass { get; }


    private Dictionary<string, IBaseSkill> _playerSkill;

    public virtual void InitStart()
    {
        _playerSkill = Managers.SkillManager.AllSKillDict
            .Where(skill => skill.Value.PlayerClass == PlayerClass)
            .ToDictionary(skill => skill.Key, skill => skill.Value);

    }

    private void Awake()
    {
        _playerSkill = new Dictionary<string, IBaseSkill>();
    }

    private void Start()
    {
        InitStart();
    }
}
