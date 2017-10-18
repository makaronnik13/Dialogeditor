using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginCanvasControler : Singleton<LoginCanvasControler> {

    public InputField usernameField, passwordField;

    public Text signText, errorText, loginText;

    private bool loginMode = true;
    public bool LoginMode
    {
        get
        {
            return loginMode;
        }
        set
        {
            if (loginMode)
            {
                signText.text = "назад ко входу";
                loginText.text = "Регистрация";
            }
            else
            {
                signText.text = "регистрация";
                loginText.text = "Вход";
            }
            loginMode = value;
        }
    }

    public void Start()
    {
        ShowCanvas();
    }

    public void OfflineModeClicked()
    {
        NetManager.Instance.Online = false;
        HideCanvas();
        QuestBookLibrary.Instance.ShowLibrary();
    }

    public void OnLoginClicked()
    {
        if (LoginMode)
        {
            switch(NetManager.Instance.Login(usernameField.text, passwordField.text))
            {
                case 0:
                    PlayerPrefs.SetString("Username", usernameField.text);
                    PlayerPrefs.SetString("Password", passwordField.text);
                    NetManager.Instance.Online = true;
                    HideCanvas();
                    QuestBookLibrary.Instance.ShowLibrary();
                    break;
                case 1:
                    errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 1);
                    errorText.text = "неправильное имя пользователя или пароль";
                    passwordField.text = "";
                    StopCoroutine(HideError());
                    StartCoroutine(HideError());
                    break;
                case 2:
                    errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 1);
                    errorText.text = "нет соединения";
                    StopCoroutine(HideError());
                    StartCoroutine(HideError());
                    break;
            }
        }
        else
        {
            if (NetManager.Instance.SignIn(usernameField.text, passwordField.text))
            {
                LoginMode = true;     
                passwordField.text = "";
            }
            else
            {
                errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, 1);
                errorText.text = "имя "+usernameField.text+" занято";
                passwordField.text = "";
                usernameField.text = "";
                StopCoroutine(HideError());
                StartCoroutine(HideError());
            }
        }
    }

    IEnumerator HideError()
    {
        while (errorText.color.a>0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, errorText.color.a-Time.deltaTime/3);
        }
    }

    public void OnSighnClicked()
    {
        LoginMode = !LoginMode;
    }

    public void ShowCanvas()
    {
        usernameField.text = "";
        passwordField.text = "";

        if (PlayerPrefs.HasKey("Username"))
        {
            usernameField.text = PlayerPrefs.GetString("Username");
            passwordField.text = PlayerPrefs.GetString("Password");
        }
        GetComponent<Animator>().SetBool("Open", true);
    }

    public void HideCanvas()
    {
        GetComponent<Animator>().SetBool("Open", false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
