using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BufferBar : UI_Scene
{
    enum BufferContextTr
    {
        BufferContext
    }


    private Transform _bufferContext;

    public Transform BufferContext { get => _bufferContext; }//���⿡ �ٸ��ֵ��� �߰��� �Ѵٸ�, 


    protected override void AwakeInit()
    {
        base.AwakeInit();
    }
    protected override void StartInit()
    {
        Bind<Transform>(typeof(BufferContextTr));
        _bufferContext = Get<Transform>((int)BufferContextTr.BufferContext);
    }




}
