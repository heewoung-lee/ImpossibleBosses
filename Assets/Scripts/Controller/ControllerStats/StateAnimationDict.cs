using System;
using System.Collections.Generic;

namespace Controller.ControllerStats
{
    public class StateAnimationDict
    {
        Dictionary<IState, Action> _stateDict = new Dictionary<IState, Action>();

        public Dictionary<IState, Action> StateDict => _stateDict;

        public void RegisterState(IState iMoveableState ,Action stateStrategy)
        {
            if (!_stateDict.ContainsKey(iMoveableState))
            {
                _stateDict.Add(iMoveableState, stateStrategy);
            }
            else
            {
                _stateDict[iMoveableState] += stateStrategy;
            }
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
}