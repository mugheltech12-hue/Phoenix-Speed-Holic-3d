using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIButtonListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action onDown;
    public Action onUp;

    public void OnPointerDown(PointerEventData eventData) => onDown?.Invoke();
    public void OnPointerUp(PointerEventData eventData) => onUp?.Invoke();
}