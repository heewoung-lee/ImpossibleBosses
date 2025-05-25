using UnityEngine;

public class LoginSceneTestTogle : MonoBehaviour
{
    void Start()
    {
        //테스트용 UI
        LogInTestToggle testTogle = Managers.UI_Manager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
    }
}
