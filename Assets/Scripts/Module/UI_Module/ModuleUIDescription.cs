using GameManagers;
using UI.Scene.SceneUI;
using UnityEngine;
using Util;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIDescription : MonoBehaviour
    {
        UIDescription _description;
        [Inject]private UIManager _uiManager; 
        public UIDescription Description
        {
            get
            {
                if(_description == null)
                {
                    _description = _uiManager.GetSceneUIFromResource<UIDescription>();
                }

                return _description;
            }
        }
        private void Start()
        {
            Description.GetComponent<Canvas>().sortingOrder = (int)Define.SpecialSortingOrder.Description;
        }
    }
}
