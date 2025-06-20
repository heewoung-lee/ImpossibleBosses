using UI;
using UI.Popup;
using UI.Scene;
using UnityEngine;

namespace GameManagers.Interface
{
    public interface IUIManager
    {
        public void SetCanvas(Canvas canvas, bool sorting = false);
        public GameObject Root { get; }

        
        
    }
}