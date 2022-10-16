using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button BtStart;
    public Button BtQuit;
    const string START = "Start";
    const string QUIT = "QUIT";
    // Start is called before the first frame update
    void Start()
    {
        EventManager.AddListner(START, LogIn);
        BtStart.onClick.AddListener(()=>{
            EventManager.CallEvent(START);
        });
        EventManager.AddListner(QUIT, QuitGame);
        BtQuit.onClick.AddListener(()=>{
            EventManager.CallEvent(QUIT);
        });
    }
    void LogIn(params object[] param){
        SceneManager.LoadScene((int)Constants.SceneName.Lobby);
    }
    void QuitGame(params object[] param)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        return;
#endif
        Application.Quit();
    }
    private void OnDestroy() {
        EventManager.RemoveListener(START, LogIn);
        EventManager.RemoveListener(QUIT, QuitGame);
    }
}
