using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region Constants and Fields

    public List<Sprite> sprites;
    [SerializeField]
    Sprite bell;
    [SerializeField]
    Sprite bomb;

    public Dictionary<bool, Sprite> shakeSprite; 
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
    void StartGame(object[] args)
    {
        btnStart.gameObject.SetActive(true);
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        EventManager.AddListener(Constants.FINISH_GAME, StartGame);
        shakeSprite = new Dictionary<bool, Sprite>();
        shakeSprite.Add(true, bomb);
        shakeSprite.Add(false, bell);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.FINISH_GAME, StartGame);   
    }
    #endregion




}
