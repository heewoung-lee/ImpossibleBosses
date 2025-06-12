using GameManagers;
using UnityEngine;

public class LoginSceneTestTogle : MonoBehaviour
{
    void Start()
    {
        //ев╫╨ф╝©К UI
        LogInTestToggle testTogle = Managers.UIManager.GetSceneUIFromResource<LogInTestToggle>(path: "Prefabs/UI/TestUI/LogInTestToggle");
    }
}
