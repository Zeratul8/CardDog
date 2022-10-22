using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    public enum PlayState{
        Wating, //시작 전
        InitGame, //패 나누는 중
        MyTurn, // 내 턴
        OtherTurn, // 상대 턴
        FinishGame, // 게임 종료 시
    }
    #region Public Constants and Fields
    public CardScript[] myCards;
    public CardScript[] otherCards;
    public bool isMyTurn = false;
    public PlayState playState = PlayState.Wating;
    public TextMeshProUGUI text;
    #endregion


    #region Private Constants and Fields

    int deckStack = 0;
    int myHand = 0;
    List<int> intList;
    List<CardClass> deckList;
    List<CardClass> basicDeck;
    WaitForSeconds waitSeconds = new WaitForSeconds(0.2f);
    List<CardClass> deck = new List<CardClass>();
    List<int> SPArr = new List<int>() { 4, 12, 16, 20, 24, 29, 32, 36, 41 };
    List<int> Light = new List<int>() { 0, 8, 28, 40, 44 };
    List<int> Blue = new List<int>() { 21, 33, 37 };
    List<int> Red = new List<int>() { 1, 5, 9 };
    List<int> Grass = new List<int>() { 13, 17, 25 };
    List<int> Bird = new List<int>() { 4, 12, 29 };
    CardClass[] myCardArr = new CardClass[10];
    #endregion

    #region Properties
    #endregion

    #region Unity Methods
    protected override void OnAwake()
    {
        EventManager.AddListener(Constants.START_GAME, ClickStartButton);
        EventManager.AddListener(Constants.PLUS_HAND, PlusHand);
        EventManager.AddListener(Constants.MINUS_CARD, MinusHand);
        EventManager.AddListener(Constants.PLAY_NEXT, PlayNextCard);

        intList = new List<int>();
        for (int i = 0; i < 50; i++)
        {
            intList.Add(i);
        }

        basicDeck = new List<CardClass>();
        SetDeck();

    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.START_GAME, ClickStartButton);
        EventManager.RemoveListener(Constants.PLUS_HAND, PlusHand);
        EventManager.RemoveListener(Constants.MINUS_CARD, MinusHand);
        EventManager.RemoveListener(Constants.PLAY_NEXT, PlayNextCard);

    }
    private void Update() {
        text.text = myHand.ToString();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Methods
    void ClickStartButton(params object[] param)
    {
        StartCoroutine(ShuffleCoroutine());
    }
    IEnumerator ShuffleCoroutine()
    {
        //셔플 애니메이션 
        yield return new WaitForSeconds(2);
        StartGame(Shuffle(), true);
    }
    List<CardClass> Shuffle()
    {
        for (int i = 0; i < intList.Count; i++)
        {
            int r = Random.Range(0, intList.Count);
            int j = intList[i];
            intList[i] = intList[r];
            intList[r] = j;
        }

        // deck.Clear();
        for (int i = 0; i < intList.Count; i++)
        {
            deck[i] = basicDeck[intList[i]];
            // deck.Add(basicDeck[intList[i]]);
        }
        return deck;
    }
    // 게임 시작.
    void StartGame(params object[] param)
    {
        playState = PlayState.InitGame;
        deckList = (List<CardClass>)param[0];
        bool isFirst = (bool)param[1];
        StartCoroutine(ShareCard(deckList, isFirst));
    }
    IEnumerator ShareCard(List<CardClass> list, bool isFirst)
    {
        //카드 깔기
        deckStack = 0;
        const int floorCard = 4;
        //바닥에 패깔기 반복문
        for (int i = 0; i < floorCard; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            EventManager.CallEvent(Constants.SET_FLOOR_CARD, list[deckStack++], false);
            yield return waitSeconds;
        }
        //내 카드 받기
        for (int i = 0; i < 5; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            // Debug.Log(myCard);
            EventManager.CallEvent(Constants.PLUS_HAND);
            yield return waitSeconds;
        }
        //상대 카드 주기
        for (int i = 0; i < 5; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            otherCards[i].SetHand(list[deckStack++], false);
            yield return waitSeconds;
        }
        //바닥에 패깔기 반복문
        for (int i = 0; i < floorCard; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            EventManager.CallEvent(Constants.SET_FLOOR_CARD, list[deckStack++], false);
            yield return waitSeconds;
        }
        //내 카드 받기
        for (int i = 5; i < 10; i++)
        {
            EventManager.CallEvent(Constants.PLUS_HAND);
            EventManager.CallEvent(Constants.POP_CARD);
            // Debug.Log(myCard);
            // myCards[i].SetHand(list[deckStack++], true);
            yield return waitSeconds;
        }
        //상대 카드 주기
        for (int i = 5; i < 10; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            otherCards[i].SetHand(list[deckStack++], false);
            yield return waitSeconds;
        }
        SortHand();
        if(isFirst) StartTurn();
        else EndTurn();
        StartCoroutine(CheckCard());
    }
    IEnumerator CheckCard()
    {
        yield return new WaitWhile(() => myHand != 0);
        EventManager.CallEvent(Constants.ZERO_HAND);
    }
    void StartTurn()
    {
        playState = PlayState.MyTurn;
        isMyTurn = true;
    }
    void EndTurn()
    {
        playState = PlayState.OtherTurn;
        isMyTurn = false;
    }
    void PlusHand(object[] param)
    {
        if (param.Length > 0)
        {
            if ((bool)param[0]) SortHand();
            Debug.Log("isJoker");
            myCards[9].SetHand(deckList[deckStack++], true);
            myHand++;
            return;
        }
        Debug.Log("not Joker");
        myCardArr[myHand] = deckList[deckStack];
        myCards[myHand++].SetHand(deckList[deckStack++], true);
    }
    void SortHand()
    {
        Dictionary<int, CardClass> indexDic = new Dictionary<int, CardClass>();
        int[] indexArr = new int[10];
        for (int i = 0; i < 10; i++)
        {
            indexArr[i] = myCardArr[i].index; 
            indexDic.Add(myCardArr[i].index, myCardArr[i]);
        }
        System.Array.Sort(indexArr);
        for(int i = 0 ; i < myCards.Length;i++){
            myCards[i].SetHand(indexDic[indexArr[i]]);
        }
    }
    void MinusHand(object[] param)
    {
        myHand--;
        // isMyTurn = false;
    }
    void PlayNextCard(object[] param){
        EventManager.CallEvent(Constants.POP_CARD);
        EventManager.CallEvent(Constants.SET_FLOOR_CARD, deckList[deckStack++], false);
    }
    void SetDeck()
    {
        for (int i = 0; i < 50; i++)
        {
            CARD_TYPE cType = CARD_TYPE.NORMAL;
            SPECIAL_CARD sCard = SPECIAL_CARD.NORMAL;
            if (SPArr.Contains(i))
            {
                cType = CARD_TYPE.SPECIAL;
            }
            else if (Light.Contains(i))
            {
                cType = CARD_TYPE.LIGHT;
            }
            else if (i == 46) cType = CARD_TYPE.BAND;

            if (Blue.Contains(i))
            {
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.BLUEBAND;
            }
            else if (Red.Contains(i))
            {
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.REDBAND;
            }
            else if (Grass.Contains(i))
            {
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.GRASSBAND;
            }
            else if (Bird.Contains(i))
            {
                sCard = SPECIAL_CARD.FIVEVBIRD;
            }


            if (i / 4 < 12)
            {
                CardClass c = new CardClass()
                {
                    index = i,
                    month = i / 4,
                    score = i == 41 || i == 47 ? 2 : 1,
                    type = cType,
                    sCard = sCard,
                };
                basicDeck.Add(c);
            }
            else
            {
                CardClass c = new CardClass()
                {
                    index = i,
                    month = -1,
                    score = 2 + i % 48,
                    type = CARD_TYPE.JOKER,
                    sCard = sCard,
                };
                basicDeck.Add(c);
            }
        }
        for (int i = 0; i < basicDeck.Count; i++)
        {
            deck.Add(basicDeck[i]);
        }
    }
    #endregion

    
}
