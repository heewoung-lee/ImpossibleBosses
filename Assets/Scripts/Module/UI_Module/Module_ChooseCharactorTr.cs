using UnityEngine;

public class Module_ChooseCharactorTr : MonoBehaviour
{
    Transform _chooseCameraTr;

    public Transform ChooseCameraTr { get => _chooseCameraTr; }

    private void Awake()
    {
        _chooseCameraTr = transform.Find("SelectCamaraTr");
    }
}
