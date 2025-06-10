using UnityEngine;

public class LoginSceneTestTogle : MonoBehaviour
{
    void Start()
    {
        //ев╫╨ф╝©К UI
        LogInTestToggle testTogle = Managers.UI_Manager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
    }
}
