using System;
using System.Collections;
using UnityEngine;

public class Module_ChoosePlayer_Manager : MonoBehaviour
{

    private const int MOVEVALUE = 4;
    private Coroutine _cameraMoveCoroutine;
    Camera _choosePlayerCamera;
    private int _currentSelectCharactorIndex = 0;
    private bool isRunnningCoroutine = false;
    private void Awake()
    {
        _choosePlayerCamera = GetComponentInChildren<Camera>();
    }

    //public void MoveSelectCamera(SelectDirection direction)
    //{
    //    if(direction == SelectDirection.LeftClick)
    //    {
    //        _currentSelectCharactorIndex--;
    //        _currentSelectCharactorIndex = Mathf.Clamp(_currentSelectCharactorIndex,0,4);
    //    }
    //    else
    //    {
    //        _currentSelectCharactorIndex++;
    //        _currentSelectCharactorIndex = Mathf.Clamp(_currentSelectCharactorIndex, 0, 4);
    //    }
    //    Vector3 moveCameraVector = new Vector3(
    //        _currentSelectCharactorIndex * MOVEVALUE,
    //        _choosePlayerCamera.transform.localPosition.y,
    //        _choosePlayerCamera.transform.localPosition.z);

    //    if (isRunnningCoroutine is true)
    //    {
    //        StopCoroutine(_cameraMoveCoroutine);
    //    }
    //    _cameraMoveCoroutine = StartCoroutine(MoveCameraLinear(moveCameraVector));
    //}

    IEnumerator MoveCameraLinear(Vector3 moveDirection)
    {
        float elapseTime = 0f;
        float DurationTime = 1f;
        isRunnningCoroutine = true;
        while (elapseTime < DurationTime)
        {
            elapseTime += Time.deltaTime;
            _choosePlayerCamera.transform.localPosition = Vector3.Lerp(_choosePlayerCamera.transform.localPosition, moveDirection, elapseTime);
            yield return null;
        }
        isRunnningCoroutine = false;
    }

}
