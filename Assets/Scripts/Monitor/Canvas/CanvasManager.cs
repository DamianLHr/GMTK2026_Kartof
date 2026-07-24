using System;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{

    [SerializeField] private GameObject loginState;
    [SerializeField] private GameObject captchaState;
    [SerializeField] private GameObject FAState;


    private Boolean loggedIn = false;
    public string Password; // to be set from big manager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

        // Activate the login screen and set the password when tab is instantiated
        changeLoginState();
        loginState.transform.GetChild(1).GetComponent<LoginController>().SetPassword(Password);
    }

    public void changeLoginState()
    {
        loginState.SetActive(true);
        captchaState.SetActive(false);
        FAState.SetActive(false);
    }

    public void changeCaptchaState() {
        loginState.SetActive(false);
        captchaState.SetActive(true);
        FAState.SetActive(false);
    }

    public void changeFAState() {
        loginState.SetActive(false);
        captchaState.SetActive(false);
        FAState.SetActive(true);
    }

}
