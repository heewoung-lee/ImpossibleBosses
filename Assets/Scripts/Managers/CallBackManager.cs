using System;
using System.Collections.Generic;

public class CallBackManager : IManagerInitializable
{
    Dictionary<int, Action> VoidCallbacks = new Dictionary<int, Action>();
    
    //TODO : ���� ���� ������Ʈ Ǯ���� �̿��ؼ� Path�� ������Ʈ Ǯ���� ��ųʸ��� �־���µ�.
    //Ŭ���̾�Ʈ���� �ѹ��� ������ ���غ��� PATH�� ����� �ȵ�

    public void regesterCallback(int callbackIndex,Action callbackAction)
    {

    }

    public void Init()
    {

    }



}