using System;
using Unity.Burst.Intrinsics;
using UnityEngine;

public interface IIndicatorBahaviour
{
    public void SetValue(float radius, float arc, Transform targetTr, Action indicatorDoneEvent = null);
    public void SetValue(float radius, float arc, Vector3 targetPos, Action indicatorDoneEvent = null);

    public float Radius { get; }
    public float Angle { get; }
    public float Arc { get;  }
    public Vector3 CallerPosition { get;}
}
