using UnityEngine;
using UnityEngine.EventSystems; 

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = transform.GetComponent<RectTransform>();
        
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas == null)
        {
            Debug.LogError("DraggableUI needs to be on an object inside a Canvas!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}