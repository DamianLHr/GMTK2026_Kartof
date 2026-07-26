using UnityEngine;

/// <summary>
/// IInteractable that toggles a UI element (e.g., panel, window, pop-up) on interaction.
/// Unlocks the cursor and disables player movement while active, then restores them when closed.
/// </summary>
public class ToggleUIInteract : MonoBehaviour, IInteractable
{
    [Header("UI Element")]
    [SerializeField] private GameObject uiTarget;

    [Header("Optional References")]
    [SerializeField] private MovementController3D movementController;

    [Header("Cursor Settings")]
    [SerializeField] private bool manageCursor = true;

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
            OpenUI();
        }
        else
        {
            CloseUI();
        }
    }

    public void OpenUI()
    {
        isActive = true;

        if (uiTarget != null)
            uiTarget.SetActive(true);

        if (movementController != null)
            movementController.enabled = false;

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void CloseUI()
    {
        isActive = false;

        if (uiTarget != null)
            uiTarget.SetActive(false);

        if (movementController != null)
            movementController.enabled = true;

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}