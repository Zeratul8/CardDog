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
    int jokerMonth = 0;
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
        EventManager.AddListener(Constants.READY_TO_PLAY, SetReady);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.SET_FLOOR_CARD, SetFloorCard);
        EventManager.RemoveListener(Constants.POP_CARD, PopCard);
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
        EventManager.RemoveListener(Constants.ZERO_HAND, ZeroHand);
        EventManager.RemoveListener(Constants.READY_TO_PLAY, SetReady);
    }
    #endregion

    #region Public Methods
    //Start ��ư���� ȣ��.
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
        
        if (month < 0)
        {
            month = preMonth;
            jokerMonth = month;
        }
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
            case PlayerManager.PlayState.InitGame:
                if(data.month < 0)
                {
                    jokerQueue.Enqueue(data);
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
            if(PlayerManager.Instance.playState != PlayerManager.PlayState.InitGame)
            {
                StartCoroutine(AddPoint(true));
            }
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
    void SetReady(params object[] param)
    {
        bool isMine = (bool)param[0];
        StartCoroutine(AddPoint(isMine));
    }
    IEnumerator AddPoint(bool isMine){
        yield return new WaitForSeconds(0.5f);
        while(jokerQueue.Count > 0)
        {
            CardClass card = jokerQueue.Dequeue();
            for (int i = 0; i < floorActive[jokerMonth]; i++)
            {
                if (floorCards[jokerMonth][i].GetCardClass().Equals(card))
                {
                    card = floorCards[jokerMonth][i].RemoveCard();
                    ScoreManager.Instance.AddPoint(card, isMine);
                    for(int j = i + 1; j < floorActive[jokerMonth]; j++)
                    {
                        CardClass cardClass = floorCards[jokerMonth][j].RemoveCard();
                        floorCards[jokerMonth][j - 1].SetCard(cardClass);
                    }
                }

            }
            floorActive[jokerMonth]--;

        }
        CardClass cc;
        while (playedCardQueue.Count > 0)
        {
            CardClass card = playedCardQueue.Dequeue();
            bool isSameMonth = playedCardQueue.Count > 0 && playedCardQueue.Peek().month == card.month;
            switch (floorActive[card.month] - jokerQueue.Count)
            {
                case 1:
                    Debug.Log("�� ����");
                    break;
                case 2:
                    for (int i = 0; i < floorActive[card.month]; i++)
                    {
                        cc = floorCards[card.month][i].RemoveCard();
                        ScoreManager.Instance.AddPoint(cc, isMine);
                    }
                    if (isSameMonth)
                    {
                        Debug.Log("��");
                        TakeACard();
                    }
                    floorActive[card.month] = 0;
                    Debug.Log("ī�� �Ա�");
                    break;
                case 3:
                    if (isSameMonth)
                    {
                        Debug.Log("��");
                        ScoreManager.Instance.SetState(ScoreState.Poop, true);
                        //PassACard();
                        playedCardQueue.Clear();
                    }
                    else
                    {
                        yield return StartCoroutine(SelectCard(card.month));
                        cc = floorCards[card.month][2].RemoveCard();
                        ScoreManager.Instance.AddPoint(cc, isMine);
                        floorActive[card.month] -= 2;
                        Debug.Log("���� ����");
                    }
                    break;
                case 4:
                    if (isSameMonth)
                    {
                        Debug.Log("����");
                    }
                    else
                    {
                        Debug.Log("�� �Ա�.");
                    }
                    TakeACard();
                    for (int i = 0; i < floorActive[card.month]; i++)
                    {
                        cc = floorCards[card.month][i].RemoveCard();
                        ScoreManager.Instance.AddPoint(cc);
                    }
                    floorActive[card.month] = 0;
                    break;
            }
            if (isSameMonth) break;
        }
    }
    void TakeACard(){
        Debug.Log("�ǻ������.");
        //������ ��û�ϱ� or �̺�Ʈ ó��.
    }
    void PassACard()
    {
        CardClass card = ScoreManager.Instance.PassCard();
        if (card == null) return;
        //������ ����.
        ScoreManager.Instance.AddPoint(card, false);
    }
    IEnumerator SelectCard(int month){
        //�˾� ����� �����ϱ�.
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