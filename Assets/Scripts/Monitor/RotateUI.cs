using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JigsawPiece : MonoBehaviour, IInteractable
{
    [SerializeField] private RectTransform rectTransform;
    public int jigsawState;
    private int correctState;

    void Awake()
    {
        jigsawState = Random.Range(0, 4);
        rectTransform.localRotation = Quaternion.Euler(0, 0, jigsawState * 90f);
    }
        
    public void OnInteraction()
    {
        Debug.Log("Interacted");
        if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
        {
            jigsawState++;
            jigsawState = jigsawState % 4;
            rectTransform.localRotation = Quaternion.Euler(0f, 0f, jigsawState * 90f);
        }
    }
    
}
