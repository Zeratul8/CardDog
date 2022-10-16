using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class RoomObjectScript : MonoBehaviour
{
    Button button;
    int index;
    // Start is called before the first frame update
    public void Init(int i)
    {
        index = i;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    void OnClick(){
        Debug.Log(index.ToString());
        SceneManager.LoadScene(((int)Constants.SceneName.Game));
    }
}
