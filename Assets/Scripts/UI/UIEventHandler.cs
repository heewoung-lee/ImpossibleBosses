using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;

namespace UI
{
    public class UIEventHandler : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Action<PointerEventData> _onLeftClickEvent;
        private Action<PointerEventData> _onRightClickEvent;
        private Action<PointerEventData> _onBeginDragEvent;
        private Action<PointerEventData> _onDragEvent;
        private Action<PointerEventData> _onEndDragEvent;
        private Action<PointerEventData> _onPointerEnterEvent;
        private Action<PointerEventData> _onPointerExitEvent;
        
        public event Action<PointerEventData> OnLeftClickEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onLeftClickEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onLeftClickEvent, value);
            }
        }
        public event Action<PointerEventData> OnRightClickEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onRightClickEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onRightClickEvent,value );
            }
        }
        public event Action<PointerEventData> OnBeginDragEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onBeginDragEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onBeginDragEvent, value);
            }
        }
        public event Action<PointerEventData> OnDragEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onDragEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onDragEvent, value);
            }
        }
        public event Action<PointerEventData> OnEndDragEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onEndDragEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onEndDragEvent, value);
            }
        }
        public event Action<PointerEventData> OnPointerEnterEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onPointerEnterEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onPointerEnterEvent, value);
            }
        }
        public event Action<PointerEventData> OnPointerExitEvent
        {
            add
            {
                UniqueEventRegister.AddSingleEvent(ref _onPointerExitEvent, value);
            }
            remove
            {
                UniqueEventRegister.RemovedEvent(ref _onPointerExitEvent, value);
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _onLeftClickEvent?.Invoke(eventData);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                _onRightClickEvent?.Invoke(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            _onBeginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            _onDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                return;

            _onEndDragEvent?.Invoke(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExitEvent?.Invoke(eventData);
        }
    }
}
