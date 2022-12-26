using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    public enum PlayState{
        Wating, //���� ��
        InitGame, //�� ������ ��
        MyTurn, // �� ��
        OtherTurn, // ��� ��
        FinishGame, // ���� ���� ��
    }
    #region Public Constants and Fields
    public CardScript[] myCards;
    public CardScript[] otherCards;
    public bool isMyTurn = false;
    public PlayState playState = PlayState.Wating;
    public Dictionary<int, bool> floorDict = new Dictionary<int, bool>();
    public List<int> floorCountList = new List<int>(monthCount);

    #endregion


    #region Private Constants and Fields
    const int monthCount = 12;
    const int HandMaxCount = 10;
    const int DeckMaxCount = 50;
    int deckStack = 0;
    int myHand = 0;
    GameObject[] myCardObject = new GameObject[HandMaxCount];
    List<Transform> initTransform;
    
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
    #endregion

    #region Properties
    #endregion

    #region Unity Methods
    protected override void OnAwake()
    {
        EventManager.AddListener(Constants.START_GAME, ClickStartButton);
        EventManager.AddListener(Constants.PLUS_HAND, PlusHand);
        EventManager.AddListener(Constants.MINUS_HAND, MinusHand);
        EventManager.AddListener(Constants.PLAY_NEXT, PlayNextCard);
        EventManager.AddListener(Constants.FINISH_GAME, FinishGame);
        intList = new List<int>();
        for(int i = 0 ; i< monthCount ; i++ ){
            floorDict.Add(i, false);
            floorCountList.Add(0);
        }
        for (int i = 0; i < DeckMaxCount; i++)
        {
            intList.Add(i);
        }

        basicDeck = new List<CardClass>(DeckMaxCount);
        initTransform = new List<Transform>(HandMaxCount);
        for(int i = 0; i < HandMaxCount; i++)
        {
            myCardObject[i] = myCards[i].gameObject;
            initTransform.Add(myCardObject[i].transform);
        }
        SetDeck();

    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.START_GAME, ClickStartButton);
        EventManager.RemoveListener(Constants.PLUS_HAND, PlusHand);
        EventManager.RemoveListener(Constants.MINUS_HAND, MinusHand);
        EventManager.RemoveListener(Constants.PLAY_NEXT, PlayNextCard);
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);

    }
    #endregion

    #region Public Methods
    public void SetBorderOff(int month){
        floorDict[month] = false;
        floorCountList[month] = 0;
    }
    public void SetBorderOn(int month){
        floorDict[month] = true;
        floorCountList[month]++;
    }

    #endregion

    #region Methods
    void FinishGame(params object[] param)
    {
        for(int i = 0; i < initTransform.Count; i++)
        {
            initTransform[i].SetAsLastSibling();
        }
        for(int i = 0 ; i<monthCount; i++){
            SetBorderOff(i);
            floorCountList[i]  = 0;
        }
    }
    void ClickStartButton(params object[] param)
    {
        StartCoroutine(ShuffleCoroutine());
    }
    IEnumerator ShuffleCoroutine()
    {
        //���� �ִϸ��̼� 
        yield return new WaitForSeconds(2);
        StartGame(Shuffle(), true);
    }
    List<CardClass> Shuffle()
    {
        // int count = intList.Count;
        // int count = DeckMaxCount;
        int count = DeckMaxCount;
        for (int i = 0; i < count; i++)
        {
            int r = Random.Range(4, count);
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
    // ���� ����.
    void StartGame(params object[] param)
    {
        playState = PlayState.InitGame;
        deckList = (List<CardClass>)param[0];
        bool isFirst = (bool)param[1];
        StartCoroutine(ShareCard(deckList, isFirst));
    }
    IEnumerator ShareCard(List<CardClass> list, bool isFirst)
    {
        //ī�� ���
        deckStack = 0;
        const int floorCard = 4;
        //�ٴڿ� �б�� �ݺ���
        for (int i = 0; i < floorCard; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            EventManager.CallEvent(Constants.SET_FLOOR_CARD, list[deckStack++], false);
            yield return waitSeconds;
        }
        if (isFirst)
        {
            //�� ī�� �ޱ�
            for (int i = 0; i < 5; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                EventManager.CallEvent(Constants.PLUS_HAND);
                yield return waitSeconds;
            }
            //��� ī�� �ֱ�
            for (int i = 0; i < 5; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                otherCards[i].SetHand(list[deckStack++], false);
                yield return waitSeconds;
            }
        }
        else
        {
            //��� ī�� �ֱ�
            for (int i = 0; i < 5; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                otherCards[i].SetHand(list[deckStack++], false);
                yield return waitSeconds;
            }
            //�� ī�� �ޱ�
            for (int i = 0; i < 5; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                EventManager.CallEvent(Constants.PLUS_HAND);
                yield return waitSeconds;
            }
        }
        //�ٴڿ� �б�� �ݺ���
        for (int i = 0; i < floorCard; i++)
        {
            EventManager.CallEvent(Constants.POP_CARD);
            EventManager.CallEvent(Constants.SET_FLOOR_CARD, list[deckStack++], false);
            yield return waitSeconds;
        }
        if (isFirst)
        {
            //�� ī�� �ޱ�
            for (int i = 5; i < 10; i++)
            {
                EventManager.CallEvent(Constants.PLUS_HAND);
                EventManager.CallEvent(Constants.POP_CARD);
                yield return waitSeconds;
            }
            //��� ī�� �ֱ�
            for (int i = 5; i < 10; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                otherCards[i].SetHand(list[deckStack++], false);
                yield return waitSeconds;
            }
        }
        else
        {
            //��� ī�� �ֱ�
            for (int i = 5; i < 10; i++)
            {
                EventManager.CallEvent(Constants.POP_CARD);
                otherCards[i].SetHand(list[deckStack++], false);
                yield return waitSeconds;
            }
            //�� ī�� �ޱ�
            for (int i = 5; i < 10; i++)
            {
                EventManager.CallEvent(Constants.PLUS_HAND);
                EventManager.CallEvent(Constants.POP_CARD);
                // Debug.Log(myCard);
                // myCards[i].SetHand(list[deckStack++], true);
                yield return waitSeconds;
            }
        }
        
        EventManager.CallEvent(Constants.READY_TO_PLAY, isFirst);
        SortHand();
        if (isFirst)
        {
            StartTurn();
        }
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
            CardScript cs = param[0] as CardScript;
            EventManager.CallEvent(Constants.POP_CARD);
            cs.SetHand(deckList[deckStack++], true);
            SortHand();
            myHand++;
            return;
        }
        
        myCards[myHand].SetHand(deckList[deckStack++], true);
        myHand++;
    }
    void SortHand()
    {
        Dictionary<int, Transform> indexDic = new Dictionary<int, Transform>();
        Dictionary<int, int> monthDict = new Dictionary<int, int>();
        for (int i = 0; i < HandMaxCount; i++)
        {
            CardClass cardClass = myCards[i].GetCardClass();
            int index = cardClass.index;
            indexDic.Add(index, myCardObject[i].transform);

            if (monthDict.ContainsKey(cardClass.month))
            {
                monthDict[cardClass.month]++;
            }
            else
            {
                monthDict.Add(cardClass.month, 1);
            }
        }
        
        indexDic = indexDic.OrderBy(item => item.Key).ToDictionary(x=>x.Key, x=>x.Value);
        foreach(Transform tr in indexDic.Values){
            tr.SetAsLastSibling();
        }
        foreach (int key in monthDict.Keys)
        {
            switch (monthDict[key])
            {
                case 4:
                //����?
                case 3:
                EventManager.CallEvent(Constants.CHECK_SHAKE, key);
                //���� ��
                    break;
                case 2:
                    if (floorCountList[key] == 2)
                    {
                        //��ź On
                        EventManager.CallEvent(Constants.CHECK_SHAKE, key);
                    }
                    break;

            }
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
        for (int i = 0; i < DeckMaxCount; i++)
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
                    type = CARD_TYPE.NORMAL,
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
