using System.Collections;
using TMPro;
using UnityEngine;

public class FAController : MonoBehaviour
{
    [SerializeField] private TMP_Text codeDisplayText;
    [SerializeField] private TMP_Text timerText;

    [Header("Timing")]
    [SerializeField] private float codeLifetime = 30f;
    [SerializeField] private float cooldownDuration = 5f;

    [Header("Cooldown Display")]
    [SerializeField] private string cooldownMessage = "Cooldown...";

    private int Code;
    private bool isOnCooldown = false;

    private EventBinding<TwoFactorSubmitEvent> twoFactorBinding;

    private void OnEnable()
    {
        twoFactorBinding = new EventBinding<TwoFactorSubmitEvent>(OnTwoFactorSubmit);
        EventBus<TwoFactorSubmitEvent>.Register(twoFactorBinding);

        GenerateNewCode();
        StartCoroutine(CodeLifecycle());
    }

    private void OnDisable()
    {
        EventBus<TwoFactorSubmitEvent>.Deregister(twoFactorBinding);
        StopAllCoroutines();
    }

    public void SetCode(int code)
    {
        Code = code;

        if (codeDisplayText != null)
        {
            codeDisplayText.text = Code.ToString();
        }
    }

    private void GenerateNewCode()
    {
        int newCode = Random.Range(10, 100); // 10–99 inclusive
        SetCode(newCode);
        EventBus<FAnumberChosenEvent>.Raise(new FAnumberChosenEvent { Number = newCode });
    }

    private IEnumerator CodeLifecycle()
    {
        while (true)
        {
            // Active phase: code is valid, timer counts down
            isOnCooldown = false;
            float remaining = codeLifetime;

            while (remaining > 0f)
            {
                if (timerText != null)
                {
                    timerText.text = Mathf.CeilToInt(remaining).ToString();
                }

                remaining -= Time.deltaTime;
                yield return null;
            }

            // Cooldown phase: code invalid, generate a new one after the wait
            isOnCooldown = true;

            if (codeDisplayText != null)
            {
                codeDisplayText.text = cooldownMessage;
            }

            float cooldownRemaining = cooldownDuration;
            while (cooldownRemaining > 0f)
            {
                if (timerText != null)
                {
                    timerText.text = Mathf.CeilToInt(cooldownRemaining).ToString();
                }

                cooldownRemaining -= Time.deltaTime;
                yield return null;
            }

            // Generate new code and go active again
            GenerateNewCode();
        }
    }

    private void OnTwoFactorSubmit(TwoFactorSubmitEvent e)
    {
        if (isOnCooldown)
        {
            Debug.Log("FA code submitted during cooldown, ignoring.");
            return;
        }

        Debug.Log($"TwoFactorListener: received code {e.Code}");

        if (e.Code == Code)
        {
            Debug.Log("FA code correct");
            EventBus<FAcodeCorrectCheckEvent>.Raise(new FAcodeCorrectCheckEvent());
        }
        else
        {
            Debug.Log("FA code incorrect");
        }
    }
}