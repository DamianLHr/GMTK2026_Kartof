using UnityEngine;

public class BootUpDialogue : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    private bool spoken = false;
    private void Update()
    {
        Debug.Log("hello");

        if (FocusInteract.focused && !spoken)
        {
            spoken = true;
            dialogueBox.Say("My password.... 'smartie'! I am too tired for this...");
        }
    }

}
