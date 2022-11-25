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
    Queue<CardClass> playedCardQueue;
    Queue<CardClass> jokerQueue;
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
        playedCardQueue = new Queue<CardClass>();
        jokerQueue = new Queue<CardClass>();
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
    //Start πˆ∆∞¿∏∑Œ »£√‚.
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
        switch (PlayerManager.Instance.playState)
        {
            case PlayerManager.PlayState.MyTurn:
                if (data.month < 0)
                { jokerQueue.Enqueue(data); }
                else
                {
                    playedCardQueue.Enqueue(data);
                }
                break;
        }

        floorCards[month][floorActive[month]].SetFloor(data);
        floorActive[month]++;
        if (isPlaying || data.month < 0)
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
        Debug.Log(jokerQueue.Count);
        CardClass cc;
        while (playedCardQueue.Count > 0)
        {
            CardClass card = playedCardQueue.Dequeue();
            bool isSameMonth = playedCardQueue.Count > 0 && playedCardQueue.Peek().month == card.month;
            Debug.Log(isSameMonth.ToString());
            switch (floorActive[card.month] - jokerQueue.Count)
            {
                case 1:
                    Debug.Log("∆– «—¿Â");
                    break;
                case 2:
                    for (int i = 0; i < floorActive[card.month]; i++)
                    {
                        cc = floorCards[card.month][i].RemoveCard();
                        ScoreManager.Instance.AddPoint(cc);
                    }
                    floorActive[card.month] = 0;
                    if(isSameMonth){
                         Debug.Log("¬ ");
                         GetOtherCard();
                         }
                    Debug.Log("ƒ´µÂ ∏‘±‚");
                    break;
                case 3:
                    if (isSameMonth)
                    {
                        Debug.Log("ª∂");
                    }
                    else
                    {
                        yield return StartCoroutine(SelectCard(card.month));
                        cc = floorCards[card.month][2].RemoveCard();
                        ScoreManager.Instance.AddPoint(cc);
                        Debug.Log("«—¿Â ∞Ì∏£±‚");
                    }
                    break;
                case 4:
                    if (isSameMonth)
                    {
                        Debug.Log("µ˚¥⁄");
                    }
                    else
                    {
                        Debug.Log("ª∂ ∏‘±‚.");
                        GetOtherCard();
                        for (int i = 0; i < floorActive[card.month]; i++)
                        {
                            cc = floorCards[card.month][i].RemoveCard();
                            ScoreManager.Instance.AddPoint(cc);
                        }
                    }
                    break;
            }
            if (isSameMonth) break;
        }
    }
    void GetOtherCard(){
        Debug.Log("««ªØæÓø¿±‚.");
    }
    IEnumerator SelectCard(int month){
        int select = Random.Range(0,2);
        CardClass cc = floorCards[month][select].RemoveCard();
        ScoreManager.Instance.AddPoint(cc);
        switch (select)
        {
            case 0:
                cc = floorCards[month][1].RemoveCard();
                floorCards[month][0].SetFloor(cc);
                break;
            case 1:
                break;
        }
        yield return null;
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