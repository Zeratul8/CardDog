using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class PlayFabTest : MonoBehaviour
{
    #region Constants and Fields
    [SerializeField]
    TMP_InputField m_id, m_password, m_userName;
    #endregion

    #region Properties
    #endregion

    #region Public Methods
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest { Email = m_id.text, Password = m_password.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest { Email = m_id.text, Password = m_password.text, Username = m_userName.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }
    #endregion


    #region Methods
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("�α��μ���!");
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("ȸ������ ����!");
    }
    void OnRegisterFailure(PlayFabError error)
    {
        Debug.Log("ȸ������ ����!");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Unity Methods
    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "97EFE";
        }
        
    }

    #endregion

}
