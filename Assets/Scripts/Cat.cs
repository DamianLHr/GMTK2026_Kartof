using UnityEngine;

public class Cat : MonoBehaviour, IInteractable
{
    
    [Header("UI Element")]
    [SerializeField] private GameObject uiTarget;

    [Header("Optional References")]
    [SerializeField] private MovementController3D movementController;

    [Header("Cursor Settings")]
    [SerializeField] private bool manageCursor = true;

    public GameObject pickedUpCat;
    
    private bool isActive = false;

    private void Start()
    {
        if (uiTarget != null)
        {
            uiTarget.SetActive(false);
        }
    }
    public void OnInteraction()
    {
        if (!isActive)
        {
            if (movementController != null)
                movementController.enabled = true;

            if (manageCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    
    
    public void OnBlockMouse()
    {
        isActive = false;
        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void OnBlockKeyboard()
    {
        isActive = false;
        if (movementController != null)
            movementController.enabled = false;
    }
    
}
