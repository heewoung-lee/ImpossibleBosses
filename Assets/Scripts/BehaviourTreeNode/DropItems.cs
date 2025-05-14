using BehaviorDesigner.Runtime.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DropItems : Action
{

    private List<int> _timeRandom;
    private int _index;
    private bool _isCallIndex;

    float _elapseTime = 0;
    public override void OnStart()
    {
        base.OnStart();
        _index = 0;
        _isCallIndex = false;
        _timeRandom = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            _timeRandom.Add(Random.Range(1,3));
        }
    }


    public override TaskStatus OnUpdate()
    {
        if(_index >= _timeRandom.Count)
        {
            Debug.Log("성공");
            return TaskStatus.Success;
        }

        int time = _timeRandom[_index];

        if(_elapseTime >= time && _isCallIndex == false)
        {
            Debug.Log($"{_timeRandom[_index]}의 {time}호출완료");
            _isCallIndex = true;
            _elapseTime = 0;
        }

        _elapseTime += Time.deltaTime;
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
