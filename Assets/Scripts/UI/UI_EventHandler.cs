using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
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
            Debug.Log("Left click detected on: " + gameObject.name);
            onLeftClickEvent?.Invoke(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click detected on: " + gameObject.name);
            onRightClickEvent?.Invoke(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        Debug.Log("Begin drag detected on: " + gameObject.name);
        onBeginDragEvent?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        Debug.Log("Dragging detected on: " + gameObject.name);
        onDragEvent?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        Debug.Log("End drag detected on: " + gameObject.name);
        onEndDragEvent?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered: " + gameObject.name);
        onPointerEnterEvent?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited: " + gameObject.name);
        onPointerExitEvent?.Invoke(eventData);
    }

}
