using System;
using System.Collections.Generic;

public class StateDictionary
{
    Dictionary<Type, Action> _stateDict = new Dictionary<Type, Action>();

    public void RegisterState<T>(Action stateStrategy)
    {
        _stateDict[typeof(T)] = stateStrategy;
    }
    public void CallState(Type type)
    {
        if (_stateDict.TryGetValue(type, out var strategy))
        {
            strategy?.Invoke();
        }
        else
        {
            Console.WriteLine($"[{type.Name}] NOT RegisteredState");
        }
    }

}