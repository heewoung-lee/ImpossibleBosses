using System;
using System.Collections.Generic;

public class CallBackManager : IManagerInitializable
{
    Dictionary<int, Action> VoidCallbacks = new Dictionary<int, Action>();
    
    //TODO : 현재 문제 오브젝트 풀링을 이용해서 Path를 오브젝트 풀링의 딕셔너리에 넣어놨는데.
    //클라이언트들이 한번도 실행을 안해봐서 PATH가 등록이 안됨

    public void regesterCallback(int callbackIndex,Action callbackAction)
    {

    }

    public void Init()
    {

    }



}