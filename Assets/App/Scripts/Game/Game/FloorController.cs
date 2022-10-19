using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    public Transform deckParent;
    public GameObject deckObject;
    public Transform cardParent;
    public GameObject cardObject;
    List<GameObject> deck;
    Dictionary<int, List<CardScript>> floorCards;
    List<int> floorActive;
    public GameUI gameUI;
    // public Dictionary<>
    int index = 0;
    int preMonth;
    // Start is called before the first frame update
    void Start()
    {
        deck = new List<GameObject>();
        EventManager.AddListener(Constants.POP_CARD, PopCard);
        EventManager.AddListener(Constants.SET_FLOOR_CARD, SetFloorCard);
    }
    private void OnDestroy() {
        EventManager.RemoveListener(Constants.SET_FLOOR_CARD, SetFloorCard);
        EventManager.RemoveListener(Constants.POP_CARD, PopCard);
    }
    //Start 버튼으로 호출.
    public void StartEvent()
    {
        if (deck.Count == 0)
        {

            for (int i = 0; i < 50; i++)
            {
                GameObject go = Instantiate(deckObject, deckParent);
                deck.Add(go);
                float move = i * 0.5f;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(move, move);
                index = i;
            }
        }
        else
        {
            for (int i = 0; i < deck.Count; i++)
            {
                deck[i].SetActive(true);
                index = i;
            }
        }
        floorActive = new List<int>();
        InitFloorCard();

    }
    public void PopCard(object[] param)
    {
        if (index >= 0)
        {
        Debug.Log("Pop");
            deck[index].SetActive(false);
            index--;
        }
    }
    void InitFloorCard()
    {
        if (floorCards == null)
        {
            floorCards = new Dictionary<int, List<CardScript>>();
            for (int i = 0; i < 12 * 6; i++)
            {
                GameObject go = Instantiate(cardObject, cardParent);
                int row = i / (6 * 6);
                int stack = i % 6;
                int column = (i / 6) % 6;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(column * 150 + 15 * stack, -row * 350 - 15 * stack);
                // go.gameObject.SetActive(true);
                int month = i / 6;
                if (stack == 0)
                {
                    floorCards.Add(month, new List<CardScript>());
                    floorActive.Add(0);
                }
                CardScript sc = go.GetComponent<CardScript>();
                floorCards[month].Add(sc);
            }
        }
    }

    public void SetFloorCard(params object[] param)
    {
        CardClass data = param[0] as CardClass;
        bool isPlaying = (bool)param[1];
        int month = data.month;
        if (month < 0) month = preMonth;
        else preMonth = month;
        floorCards[month][floorActive[month]].SetFloor(data);
        floorActive[month]++;
        if(isPlaying || data.month < 0){
            StartCoroutine(PlayNext());
        }
    }
    IEnumerator PlayNext(){
        yield return new WaitForSeconds(0.5f);
            EventManager.CallEvent(Constants.PLAY_NEXT);
    }
}
