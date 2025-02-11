using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingIconView : MonoBehaviour, IPointerDownHandler
{
    public event Action<int> OnDrag;

    public int Id;
    public TextMeshProUGUI PriceText;
    public Image Image;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag?.Invoke(Id);
    }
}
