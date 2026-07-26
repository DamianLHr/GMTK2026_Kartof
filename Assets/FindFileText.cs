using UnityEngine;

public class FindFileText : MonoBehaviour
{
    [SerializeField] private DialogueBox dialogueBox;
    private bool spoken = false;

    private void Start()
    {
        dialogueBox = FindAnyObjectByType<DialogueBox>();
        
        if (dialogueBox != null)
        {
            dialogueBox.ClearBox();
            dialogueBox.Say("'''don't forget you dum dum''', where did I put that file??");
        }
    }
}