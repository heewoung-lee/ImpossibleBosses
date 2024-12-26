using System;
using System.Collections.Generic;

public class StateAnimationDict
{
    Dictionary<IState, Action> _stateDict = new Dictionary<IState, Action>();

    public void RegisterState(IState iMoveableState ,Action stateStrategy)
    {
        _stateDict[iMoveableState] = stateStrategy;
    }
    public void CallState(IState iMoveableState)
    {
        if (_stateDict.TryGetValue(iMoveableState, out var strategy))
        {
            strategy?.Invoke();
        }
        else
        {
            Console.WriteLine($"[{iMoveableState}] NOT RegisteredState");
        }
    }

}