using UnityEngine;

// Unlocks and shows the mouse cursor when this object starts.
// Drop on a menu / victory / loss scene object so the cursor is usable there.
public class FreeMouseOnStart : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
