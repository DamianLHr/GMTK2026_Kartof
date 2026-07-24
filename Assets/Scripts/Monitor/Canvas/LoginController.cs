using System.Collections;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private string Username;
    [SerializeField] private float letterDelay = 0.05f;

    [SerializeField] private CanvasManager canvasManager; // Reference to the CanvasManager script
    [Header("Feedback")]
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private string correctMessage = "Correct!";
    [SerializeField] private string incorrectMessage = "Incorrect password, try again.";
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    [SerializeField] private float successDelay = 1f;

    private string Password;

    private void Awake()
    {
        usernameField.readOnly = true;
        usernameField.text = "";

        passwordField.contentType = TMP_InputField.ContentType.Password;
        passwordField.ForceLabelUpdate();

        passwordField.onSubmit.AddListener(OnPasswordSubmit);

        feedbackText.text = "";
    }

    private void OnEnable()
    {
        StartCoroutine(TypeUsername());
    }

    private IEnumerator TypeUsername()
    {
        usernameField.text = "";
        passwordField.interactable = false; // lock password field while typing

        foreach (char c in Username)
        {
            usernameField.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        passwordField.interactable = true; // unlock once done
    }

    public void SetPassword(string password)
    {
        Password = password;
    }

    private void OnPasswordSubmit(string enteredValue)
    {
        TrySubmitLogin(enteredValue);
    }

    private void TrySubmitLogin(string enteredValue)
    {
        bool success = enteredValue == Password;

        feedbackText.text = success ? correctMessage : incorrectMessage;
        feedbackText.color = success ? correctColor : incorrectColor;

        if (success)
        {
            Debug.Log("Login success");
            passwordField.interactable = false; // lock input during the transition delay
            StartCoroutine(GoToCaptchaAfterDelay());
        }
        else
        {
            Debug.Log("Wrong password");
            passwordField.text = "";
            passwordField.ActivateInputField();
        }
    }

    private IEnumerator GoToCaptchaAfterDelay()
    {
        yield return new WaitForSeconds(successDelay);
        canvasManager.changeCaptchaState();
    }

    private void OnDisable()
    {
        feedbackText.text = "";
    }
}