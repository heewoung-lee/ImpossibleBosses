using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, 
    IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler,IPointerExitHandler
{
    public Action<PointerEventData> onLeftClickEvent;
    public Action<PointerEventData> onRightClickEvent;
    public Action<PointerEventData> onBeginDragEvent;
    public Action<PointerEventData> onDragEvent;
    public Action<PointerEventData> onEndDragEvent;
    public Action<PointerEventData> onPointerEnterEvent;
    public Action<PointerEventData> onPointerExitEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            onLeftClickEvent?.Invoke(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClickEvent?.Invoke(eventData);
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragEvent?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDragEvent?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragEvent?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterEvent?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExitEvent?.Invoke(eventData);
    }

}
