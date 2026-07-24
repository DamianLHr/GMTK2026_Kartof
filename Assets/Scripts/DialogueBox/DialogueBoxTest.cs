using UnityEngine;

public class DialogueBoxTest : MonoBehaviour
{
    [SerializeField] private DialogueBox box;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Update()
    {
        box.Say("Hello, this is a test dialogue.");

    }
}
