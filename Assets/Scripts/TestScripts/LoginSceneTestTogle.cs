using UnityEngine;

public class LoginSceneTestTogle : MonoBehaviour
{
    void Start()
    {
        //�׽�Ʈ�� UI
        LogInTestToggle testTogle = Managers.UI_Manager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
    }
}
