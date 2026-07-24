using TMPro;
using UnityEngine;

public class ChangeErrorText : MonoBehaviour
{
    public string text;
    public TextMeshProUGUI textDisplay;

    void Start()
    {
        textDisplay.text += text;
        Destroy(this);
    }
}
