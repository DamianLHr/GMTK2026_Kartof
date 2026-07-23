using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueBox : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public GameObject textBox;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float linePause = 1f;

    private string _bufferedLine = null;
    private string _currentLine = string.Empty;
    private bool _isTyping = false;

    void Start()
    {
        textBox.SetActive(false);
        textComponent.text = string.Empty;
        _isTyping = false;
    }

    public void OnAdvance(InputAction.CallbackContext context)
    {
        if (!context.performed || !_isTyping) return;
        if (_isTyping)
        {
            StopAllCoroutines();
            textComponent.text = _currentLine;
            _isTyping = false;
            StartCoroutine(PauseAndAdvance());
        }
    }

    public void Say(string line)
    {
        if (_isTyping)
        {
            _bufferedLine = line; // overwrite — only keep latest
        }
        else
        {
            StartLine(line);
        }
    }

    private void StartLine(string line)
    {
        _isTyping = true;
        _currentLine = line;
        _bufferedLine = null;
        textComponent.text = string.Empty;
        textBox.SetActive(true);
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in _currentLine)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        _isTyping = false;
        StartCoroutine(PauseAndAdvance());
    }

    private IEnumerator PauseAndAdvance()
    {
        yield return new WaitForSeconds(linePause);
        if (_bufferedLine != null)
            StartLine(_bufferedLine);
        else
            ClearBox();
    }

    public void ClearBox()
    {
        StopAllCoroutines();
        _bufferedLine = null;
        textComponent.text = string.Empty;
        textBox.SetActive(false);
        _isTyping = false;
        _currentLine = string.Empty;
    }
}