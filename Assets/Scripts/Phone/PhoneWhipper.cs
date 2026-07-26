using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PhoneWhipper : MonoBehaviour
{   

    [SerializeField] private PhoneUIController phoneUI;
    [SerializeField] private MovementController3D movementController;
    [SerializeField] private SFXConfiguration whipOutSFX;

    [SerializeField] private SFXConfiguration secretWhipSFX;

    private bool phoneOpen = false;

    public void OnTogglePhone(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        int rand = UnityEngine.Random.Range(0, 100);
        if(rand <= 10)
        {
            AudioManager.Instance.PlaySFX(secretWhipSFX, 1f);
        } else
        {
            AudioManager.Instance.PlaySFX(whipOutSFX, 1f);
        }
        if (!phoneOpen)
        {
            if (!movementController.enabled) return; // refuse to open if movement is already locked (e.g. FocusInteract active)
            Debug.Log("whipped that shit out");
            phoneOpen = true;
        }
        else
        {
            phoneOpen = false;
        }

        phoneUI.TogglePhone(phoneOpen);
        movementController.enabled = !phoneOpen;
    }

        public void debug2FAEvent(InputAction.CallbackContext context)
    {   
        if (!context.performed) return;
        //EventBus<Phone2FAEvent>.Raise(new Phone2FAEvent{correctNumber = 42});
        Debug.Log("2FA number event raised");
    }
}
