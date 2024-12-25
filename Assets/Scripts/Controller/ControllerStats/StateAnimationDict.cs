using System;
using System.Collections.Generic;

public class StateAnimationDict
{
    Dictionary<IMoveableState, Action> _stateDict = new Dictionary<IMoveableState, Action>();

    public void RegisterState(IMoveableState iMoveableState ,Action stateStrategy)
    {
        _stateDict[iMoveableState] = stateStrategy;
    }
    public void CallState(IMoveableState iMoveableState)
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