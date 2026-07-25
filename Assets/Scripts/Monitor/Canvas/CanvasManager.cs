using System;
using System.Collections;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject loginState;
    [SerializeField] private GameObject captchaState;
    [SerializeField] private GameObject FAState;
    [SerializeField] private GameObject submitState;

    [SerializeField] private float stateChangeDelay = 0.5f;
    private float instantChange = 0;

    private Boolean loggedIn = false;
    public string Password; // to be set from big manager;
    public int FAcode; // to be set from big manager;

    private EventBinding<PasswordCorrectCheckEvent> passwordCorrectBinding;
    private EventBinding<CaptchaSolvedEvent> captchaSolvedBinding;
    private EventBinding<FAcodeCorrectCheckEvent> faCodeCorrectBinding;

    private void Awake()
    {
        loginState.GetComponent<LoginController>().SetPassword(PuzzleOrchestrator.CanvasPassword);
        FAState.GetComponent<FAController>().SetCode(PuzzleOrchestrator.FACode);
        
        if(PuzzleOrchestrator.FACodeCorrect) { changeSubmitState(instantChange); }
        else if(PuzzleOrchestrator.CaptchaCorrect) { changeFAState(instantChange); }
        else if(PuzzleOrchestrator.PasswordCorrect) { changeCaptchaState(instantChange); }
        else { changeLoginState(instantChange); }
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
        PuzzleOrchestrator.PasswordCorrect = true;
        loggedIn = true;
        changeCaptchaState(stateChangeDelay);
    }

    private void OnCaptchaSolved()
    {
        PuzzleOrchestrator.CaptchaCorrect = true;
        changeFAState(stateChangeDelay);
    }

    private void OnFACodeCorrect()
    {
        PuzzleOrchestrator.FACodeCorrect = true;
        changeSubmitState(stateChangeDelay);
    }

    public void changeLoginState(float seconds)
    {
        StartCoroutine(DelayedChange(() =>
        {
            loginState.SetActive(true);
            captchaState.SetActive(false);
            FAState.SetActive(false);
            submitState.SetActive(false);
        }, seconds));
    }

    public void changeCaptchaState(float seconds)
    {
        StartCoroutine(DelayedChange(() =>
        {
            loginState.SetActive(false);
            captchaState.SetActive(true);
            FAState.SetActive(false);
            submitState.SetActive(false);
        }, seconds));
    }

    public void changeFAState(float seconds)
    {
        StartCoroutine(DelayedChange(() =>
        {
            loginState.SetActive(false);
            captchaState.SetActive(false);
            FAState.SetActive(true);
            submitState.SetActive(false);
        }, seconds));
    }

    public void changeSubmitState(float seconds)
    {
        StartCoroutine(DelayedChange(() =>
        {
            loginState.SetActive(false);
            captchaState.SetActive(false);
            FAState.SetActive(false);
            submitState.SetActive(true);
        }, seconds));
    }

    private IEnumerator DelayedChange(Action applyChange, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        applyChange.Invoke();
    }
}