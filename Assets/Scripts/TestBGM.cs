using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBGM : MonoBehaviour
{

    void Start()
    {
        Managers.SoundManager.Play("Sounds/001.BGM", Define.Sound.BGM);
    }


}
