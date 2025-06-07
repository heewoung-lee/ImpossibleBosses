using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BufferBar : UI_Scene,ISceneChangeBehaviour
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
    private void OnEnable()
    {
        Managers.SceneManagerEx.OnBeforeSceneUnloadLocalEvent += OnBeforeSceneUnload;
    }

    private void OnDisable()
    {
        Managers.SceneManagerEx.OnBeforeSceneUnloadLocalEvent -= OnBeforeSceneUnload;
    }



    protected override void StartInit()
    {
        Bind<Transform>(typeof(BufferContextTr));
        _bufferContext = Get<Transform>((int)BufferContextTr.BufferContext);
    }

    public void OnBeforeSceneUnload()
    {
        foreach(BufferComponent buffer in _bufferContext.GetComponentsInChildren(typeof(BufferComponent)))
        {
            Managers.BufferManager.RemoveBuffer(buffer);
        }
    }
}
