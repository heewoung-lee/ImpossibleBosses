using UnityEngine;

namespace Module.UI_Module
{
    public class ModuleChooseCharactorTr : MonoBehaviour
    {
        Transform _chooseCameraTr;

        public Transform ChooseCameraTr { get => _chooseCameraTr; }

        private void Awake()
        {
            _chooseCameraTr = transform.Find("SelectCamaraTr");
        }
    }
}
