using GameManagers;
using GameManagers.Interface.UIManager;
using UI.Scene.SceneUI;
using UnityEngine;
using Util;
using Zenject;

namespace Module.UI_Module
{
    public class ModuleUIDescription : MonoBehaviour
    {
        UIDescription _description;
        [Inject]private IUISceneManager _uisceneManager; 
        public UIDescription Description
        {
            get
            {
                if(_description == null)
                {
                    _description = _uisceneManager.GetSceneUIFromResource<UIDescription>();
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
