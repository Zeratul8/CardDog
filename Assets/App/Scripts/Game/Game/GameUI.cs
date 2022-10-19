using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region Constants and Fields

    public List<Sprite> sprites;
    public Button btnStart;

    #endregion

    #region Properties
    #endregion

    #region Public Methods
    public void ClickStart()
    {
        Debug.Log("Start");
        EventManager.CallEvent(Constants.START_GAME);
    }
    public void ClickExit(){
        Debug.Log("Exit");
        SceneManager.LoadScene((int)Constants.SceneName.Lobby);
    }
    #endregion

    #region Methods
    
    #endregion

    #region Unity Methods
    private void Awake()
    {
        EventManager.AddListener(Constants.FINISH_GAME, (object[] args) =>
        {
            btnStart.gameObject.SetActive(true);
        });
    }
    #endregion



    
}
