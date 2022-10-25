using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloorController : MonoBehaviour
{
    #region Constants and Fields
    public Transform deckParent;
    public GameObject deckObject;
    public Transform cardParent;
    public GameObject cardObject;
    List<GameObject> deck;
    Dictionary<int, List<CardScript>> floorCards;
    List<int> floorActive;
    Queue<CardClass> cardQueue;
    public GameUI gameUI;
    const int deckCount = 50;
    const int monthCount = 12;
    const int cellCount = 6;
    

    int index = 0;
    int preMonth;
    bool isFinish = false;
    #endregion

    #region Properties
    #endregion

#region Unity Methods
    // Start is called before the first frame update
    void Start()
    {
        deck = new List<GameObject>();
        cardQueue = new Queue<CardClass>();
        EventManager.AddListener(Constants.POP_CARD, PopCard);
        EventManager.AddListener(Constants.SET_FLOOR_CARD, SetFloorCard);
        EventManager.AddListener(Constants.FINISH_GAME, FinishGame);
        EventManager.AddListener(Constants.ZERO_HAND, ZeroHand);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.SET_FLOOR_CARD, SetFloorCard);
        EventManager.RemoveListener(Constants.POP_CARD, PopCard);
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
        EventManager.RemoveListener(Constants.ZERO_HAND, ZeroHand);
    }
    #endregion

    #region Public Methods
    //Start 버튼으로 호출.
    public void StartEvent()
    {
        if (deck.Count == 0)
        {

            for (int i = 0; i < deckCount; i++)
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
        if(floorActive == null) floorActive = new List<int>();
        InitFloorCard();

    }
    public void PopCard(object[] param)
    {
        if (index >= 0)
        {
            deck[index].SetActive(false);
            index--;
        }
    }
    #endregion

    #region Methods
    
    void InitFloorCard()
    {
        if (floorCards == null)
        {
            floorCards = new Dictionary<int, List<CardScript>>();
            for (int i = 0; i < monthCount * cellCount; i++)
            {
                GameObject go = Instantiate(cardObject, cardParent);
                const int columnCount = 6;
                int row = i / (cellCount * columnCount);
                int stack = i % cellCount;
                int column = (i / cellCount) % columnCount;
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(column * 150 + 15 * stack, -row * 350 - 15 * stack);
                // go.gameObject.SetActive(true);
                int month = i / cellCount;
                if (stack == 0)
                {
                    floorCards.Add(month, new List<CardScript>());
                    floorActive.Add(0);
                }
                CardScript sc = go.GetComponent<CardScript>();
                floorCards[month].Add(sc);
            }
        }
        isFinish = false;
    }

    public void SetFloorCard(params object[] param)
    {

        CardClass data = param[0] as CardClass;
        bool isPlaying = (bool)param[1];
        int month = data.month;
        if (month < 0) month = preMonth;
        else preMonth = month;
        if(PlayerManager.Instance.playState == PlayerManager.PlayState.MyTurn) cardQueue.Enqueue(data);
    

        floorCards[month][floorActive[month]].SetFloor(data);
        floorActive[month]++;
        if(isPlaying && data.month < 0){
            // 내 점수 판으로 들어오기.
            // 새 카드 주기.
            EventManager.CallEvent(Constants.PLUS_HAND, true);
        }
        else if (isPlaying || data.month < 0)
        {
            StartCoroutine(PlayNext());
        }
        else{
            StartCoroutine(AddPoint());
        }
    }
    public void FinishGame(params object[] param){
        for(int i = 0 ; i<monthCount ; i++){
            floorActive[i] = 0;
        }
        for (int i = 0; i < deckCount; i++)
        {
            deck[i].SetActive(false);
        }

    }
    void ZeroHand(params object[] param)
    {
        isFinish = true;
    }
    IEnumerator AddPoint(){
        yield return new WaitForSeconds(0.5f);
        while (cardQueue.Count > 0)
        {
            
            CardClass card = cardQueue.Dequeue();

            Debug.Log(card.month);
            Debug.Log(card.index);
            if (card.month < 0)
            {
                Debug.Log("조커는 패스");
            }
            else
            {
                switch (floorActive[card.month])
                {
                    case 1:
                        Debug.Log("패 한장");
                        break;
                    case 2:
                        for (int i = 0; i < floorActive[card.month]; i++)
                        {
                            CardClass cc = floorCards[card.month][i].RemoveCard();
                            ScoreManager.Instance.AddPoint(cc);
                        }
                        floorActive[card.month] = 0;
                        Debug.Log("카드 먹기");
                        break;
                    case 3:
                        Debug.Log("뻑? or 한장 고르기");
                        break;
                    case 4:
                        Debug.Log("따닥? or 뻑 먹기.");
                        break;
                }
            }
        }
    }
    IEnumerator PlayNext()
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.CallEvent(Constants.PLAY_NEXT);

        if (isFinish)
        {
            yield return new WaitForSeconds(1);
            EventManager.CallEvent(Constants.FINISH_GAME);
        }
    }
    #endregion
}