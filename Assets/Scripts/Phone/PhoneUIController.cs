using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PhoneUIController : MonoBehaviour
{
    [Header("Phone Slide")]
    [SerializeField] private RectTransform phonePanel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 shownPosition;
    [SerializeField] private float slideDuration = 0.3f;

    [Header("Code Panel")]
    [SerializeField] private GameObject codePanel;                  // covers the phone screen
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private Button[] codeButtons;                  // exactly 3
    [SerializeField] private TextMeshProUGUI[] codeButtonTexts;     // matching, index-for-index

    [Header("Icon Control")]

    [SerializeField] private GameObject WideEyesCatIcon;
    [SerializeField] private GameObject BlinkingCatIcon;

    private float frame1Duration = 2.5f;
    private float frame2Duration = 0.2f;
    private Coroutine blinkingAnimation;
    private bool isAnimating = false;
    private bool isPhoneOpen = false;
    private Coroutine slideRoutine;
    private int correctNumber = 0;
    private bool triggered2FA = false;

    EventBinding<FAnumberChosenEvent> phone2FAeventBinding;
    EventBinding<correctNumberEvent> correctNumber2FAEventBinding;

    private void Awake()
    {   
        timerPanel.SetActive(false);
        codePanel.SetActive(false);
        phonePanel.anchoredPosition = hiddenPosition; // Hidden phone on Scene Start

    }
public struct FAnumberChosenEvent : IEvent { public int Number; }
    private void OnEnable()
    {
        phone2FAeventBinding = new EventBinding<FAnumberChosenEvent>(e => handle2FAEvent(e.Number));  //Event that provides the phone with the number generated for the F2A
        EventBus<FAnumberChosenEvent>.Register(phone2FAeventBinding);
        correctNumber2FAEventBinding = new EventBinding<correctNumberEvent>(() => Debug.Log("correct number chosen")); //Debugging, might be useful later
        EventBus<correctNumberEvent>.Register(correctNumber2FAEventBinding);
    }

    private void OnDisable()
    {   
        EventBus<FAnumberChosenEvent>.Deregister(phone2FAeventBinding);
        EventBus<correctNumberEvent>.Deregister(correctNumber2FAEventBinding);
    }

    public bool IsPhoneOpen => isPhoneOpen;

    public void TogglePhone(bool show)
    {
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlidePhone(show ? shownPosition : hiddenPosition));
        isPhoneOpen = show;
        WideEyesCatIcon.SetActive(true);
        BlinkingCatIcon.SetActive(false);
        SetAnimationState(!isAnimating);

        // Toggle cursor visibility and lock state
        SetCursorState(false);

        //if (!show) HideCodePanel();
        if (show && triggered2FA)
        {
            HandleShowCodePanel();
            triggered2FA = false;
        }
    }

    private IEnumerator SlidePhone(Vector2 target)
    {
        Vector2 start = phonePanel.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            phonePanel.anchoredPosition = Vector2.Lerp(start, target, elapsed / slideDuration);
            yield return null;
        }
        phonePanel.anchoredPosition = target;
    }

    private void HandleShowCodePanel()
    {
        if (!isPhoneOpen) return; // same rule the notification had — only while the phone is up
        SetCursorState(true);

        int decoyA = GenerateDecoy(correctNumber);
        int decoyB = GenerateDecoy(correctNumber, decoyA);

        var numbers = new List<int> { correctNumber, decoyA, decoyB };
        Shuffle(numbers);

        for (int i = 0; i < codeButtons.Length; i++)
        {
            int shownNumber = numbers[i];
            codeButtonTexts[i].text = shownNumber.ToString();

            codeButtons[i].onClick.RemoveAllListeners();
            codeButtons[i].onClick.AddListener(() => OnCodeButtonClicked(shownNumber));
        }

        codePanel.SetActive(true);
    }

    private void OnCodeButtonClicked(int number)
    {
        if (number == correctNumber)
        {
            EventBus<TwoFactorSubmitEvent>.Raise(new TwoFactorSubmitEvent{Code = correctNumber});
            Debug.Log("Correct number was chosen");
            HideCodePanel();
            triggered2FA = false;
            SetCursorState(false);
        }
        else
        {
            // Wait 5 seconds if the wrong button is pressed
            StartCoroutine(Countdown(5));
        }
    }

    private void HideCodePanel()
    {
        codePanel.SetActive(false);
    }

    private int GenerateDecoy(params int[] excluding)
    {
        int candidate;
        do
        {
            candidate = UnityEngine.Random.Range(0, 100); // range guess — adjust to whatever CurrentNumber's real range is
        } while (Array.IndexOf(excluding, candidate) >= 0);
        return candidate;
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private void handle2FAEvent(int number)
    {
        triggered2FA = true;
        correctNumber = number;
        if (isPhoneOpen)
        {
            HandleShowCodePanel();
            triggered2FA = false;
        }
    }

    private void SetCursorState(bool freeCursor)
{
    if (freeCursor)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    else
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

    private IEnumerator Countdown(float time)
    {   
        SetButtonsInteractable(false);
        codePanel.SetActive(false);
        timerPanel.SetActive(true);
        float timeRemaining = time;
            while (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                if (statusText != null)
            {
                statusText.text = $"Wrong code! Locked for {Mathf.CeilToInt(timeRemaining)}s...";
            }
                yield return null; // Wait until next frame
            }
        Debug.Log("Penalty over");

        // 3. Reset text & re-enable buttons after 5 seconds
        if (statusText != null) statusText.text = "";
        timerPanel.SetActive(false);
        codePanel.SetActive(true);
        SetButtonsInteractable(true);
        
    }

    private void SetButtonsInteractable(bool state)
{
    foreach (var btn in codeButtons)
    {
        if (btn != null) btn.interactable = state;
    }
}

    private void SetAnimationState(bool state)
    {
        if (isAnimating == state) return;

        isAnimating = state;

        if (isAnimating)
        {
            blinkingAnimation = StartCoroutine(frameSwitch());
        } else if (blinkingAnimation != null)
        {
            StopCoroutine(frameSwitch());
        }
    }

    private IEnumerator frameSwitch()
    {
        while (isAnimating)
        {
            WideEyesCatIcon.SetActive(true);
            BlinkingCatIcon.SetActive(false);
            yield return new WaitForSeconds(frame1Duration);
            BlinkingCatIcon.SetActive(true);
            WideEyesCatIcon.SetActive(false);
            yield return new WaitForSeconds(frame2Duration);
        }
    }

}