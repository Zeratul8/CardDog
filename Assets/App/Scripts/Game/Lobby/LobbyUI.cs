using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public GameObject roomObject;
    public RectTransform contentRect;
    public float padding;
    public float primaryPadding;
    public int roomCount;
    //.xml파일이나 .json 등으로 게임 배팅 폼 만들기.
    List<string> gameList = new List<string>(){
        "자유경기장",
        "점당\n천원",
        "점당\n오천원",
        "점당\n만원",
        "점당\n삼만원",
        "점당\n오만원",
        "점당\n십만원",
        "점당\n오십만원",
        "점당\n백만원",
        "점당\n천만원",
    };
    // Start is called before the first frame update
    void Start()
    {
        RectTransform objectRect = roomObject.GetComponent<RectTransform>();
        float width = objectRect.rect.width;
        contentRect.sizeDelta = new Vector2(primaryPadding * 2 + roomCount * (width + padding), contentRect.rect.height);
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = Instantiate(roomObject, contentRect);
            RectTransform roomRect = go.GetComponent<RectTransform>();
            roomRect.anchorMin = new Vector2(0, 0.5f);
            roomRect.anchorMax = new Vector2(0, 0.5f);
            roomRect.anchoredPosition = new Vector2(primaryPadding + 0.5f*width + i * (width + padding), 0);

            TextMeshProUGUI tm = go.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = gameList[i];
            Button bt = go.GetComponent<Button>();
            bt.onClick.AddListener(()=>{
                Debug.Log("게임방 입장."+gameList[i]);
                //서버 데이터랑 던져주고 response오면 받아서 씬 이동하기.
                //이때 PlayerPrefs 쓸 건지, Don'tDestroyObject 이용할건지?
                
            });
        }
    }
}
