using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx:IManagerIResettable
{ 
 
    //���� ���̽� ���� �����´�.
    //LoadScene ���� Ŭ�������� �ε��Ѵ�.
    //Enum�� �̸����� �ٲ��ִ� �޼���
    public BaseScene currentScene { get => GameObject.FindAnyObjectByType<BaseScene>(); }

    public void LoadScene(Define.Scene scene)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetEnumName(scene));
    }

    public string GetEnumName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return name;
    }

    public void Clear()
    {
        currentScene.Clear();   
    }
}
