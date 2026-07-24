using UnityEngine;
using UnityEngine.EventSystems;
public class RotateUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler

{
    [SerializeField] private RectTransform rectTransform;
    public int jigsawState;
    public int correctState;
    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
    
    void Awake()
    {
        jigsawState = Random.Range(0, 4);
        rectTransform.localRotation = Quaternion.Euler(0, 0, jigsawState * 90f);
    }
    
    void Update()
    {
        if (isHovering && Input.GetKeyDown(KeyCode.R))
        {
            jigsawState++;
            jigsawState = jigsawState % 4;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, jigsawState * 90f);
        }
    }
    
}
