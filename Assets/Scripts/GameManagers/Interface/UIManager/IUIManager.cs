using UnityEngine;

namespace GameManagers.Interface.UI_Interface
{
    public interface IUIManager
    {
        public void SetCanvas(Canvas canvas, bool sorting = false);
        public GameObject Root { get; }

        
        
    }
}