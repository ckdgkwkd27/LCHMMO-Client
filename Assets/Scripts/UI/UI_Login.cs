using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Diagnostics;

public class UI_Login : MonoBehaviour
{
    public InputField idField;
    public InputField pwField;
    public Button joinButton;
    public Button loginButton;
    private UnityAction action;

    public void Start()
    {
        
    }

    public void OnJoinButtonClick()
    {
        string id = idField.text;
        string pw = pwField.text;
        Debug.Log($"INFO: Id={id}, pw={pw}");

        Protocol.RequestLogin requestLogin;
        requestLogin.Id = id;
        requestLogin.Password = pw;
        Managers.Network.Send(requestLogin);

    }

    void OnLoginButtonClick()
    {

    }
}