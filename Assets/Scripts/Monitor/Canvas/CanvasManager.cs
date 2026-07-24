using System;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject loginState;
    [SerializeField] private GameObject captchaState;
    [SerializeField] private GameObject FAState;

    private Boolean loggedIn = false;
    public string Password; // to be set from big manager;
    public int FAcode; // to be set from big manager;

    private EventBinding<PasswordCorrectCheckEvent> passwordCorrectBinding;
    private EventBinding<CaptchaSolvedEvent> captchaSolvedBinding;
    private EventBinding<FAcodeCorrectCheckEvent> faCodeCorrectBinding;

    private void Awake()
    {
        changeLoginState();
        loginState.GetComponent<LoginController>().SetPassword(Password);
        FAState.GetComponent<FAController>().SetCode(FAcode);
    }

    private void OnEnable()
    {
        passwordCorrectBinding = new EventBinding<PasswordCorrectCheckEvent>(OnPasswordCorrect);
        EventBus<PasswordCorrectCheckEvent>.Register(passwordCorrectBinding);

        captchaSolvedBinding = new EventBinding<CaptchaSolvedEvent>(OnCaptchaSolved);
        EventBus<CaptchaSolvedEvent>.Register(captchaSolvedBinding);

        faCodeCorrectBinding = new EventBinding<FAcodeCorrectCheckEvent>(OnFACodeCorrect);
        EventBus<FAcodeCorrectCheckEvent>.Register(faCodeCorrectBinding);
    }

    private void OnDisable()
    {
        EventBus<PasswordCorrectCheckEvent>.Deregister(passwordCorrectBinding);
        EventBus<CaptchaSolvedEvent>.Deregister(captchaSolvedBinding);
        EventBus<FAcodeCorrectCheckEvent>.Deregister(faCodeCorrectBinding);
    }

    private void OnPasswordCorrect()
    {
        loggedIn = true;
        changeCaptchaState();
    }

    private void OnCaptchaSolved()
    {
        changeFAState();
    }

    private void OnFACodeCorrect()
    {
        Debug.Log("CanvasManager: FA code correct, proceeding.");
        // next state call goes here once you have one (e.g. assignment list)
    }

   
    public void changeLoginState()
    {
        loginState.SetActive(true);
        captchaState.SetActive(false);
        FAState.SetActive(false);
    }

    public void changeCaptchaState()
    {
        loginState.SetActive(false);
        captchaState.SetActive(true);
        FAState.SetActive(false);
    }

    public void changeFAState()
    {
        loginState.SetActive(false);
        captchaState.SetActive(false);
        FAState.SetActive(true);
    }
}