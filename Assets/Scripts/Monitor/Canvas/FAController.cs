using TMPro;
using UnityEngine;

public class FAController : MonoBehaviour
{
    [SerializeField] private TMP_Text codeDisplayText;

    private int Code;

    private EventBinding<TwoFactorSubmitEvent> twoFactorBinding;

    private void OnEnable()
    {
        twoFactorBinding = new EventBinding<TwoFactorSubmitEvent>(OnTwoFactorSubmit);
        EventBus<TwoFactorSubmitEvent>.Register(twoFactorBinding);
    }

    private void OnDisable()
    {
        EventBus<TwoFactorSubmitEvent>.Deregister(twoFactorBinding);
    }

    public void SetCode(int code)
    {
        Code = code;

        if (codeDisplayText != null)
        {
            codeDisplayText.text = Code.ToString();
        }
    }

    private void OnTwoFactorSubmit(TwoFactorSubmitEvent e)
    {
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