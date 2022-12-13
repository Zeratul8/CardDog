using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    struct StateStruct
    {
        public ScoreState state { get; }
        public string name { get; }
        public int point;
        public void AddPoint() => point++;

        public StateStruct(ScoreState _state, int _point, string _name)
        {
            state = _state;
            name = _name;
            point = _point;
        }
    }
    class StateClass
    {
        public StateStruct state;
        public TextMeshProUGUI textMesh;
        public StateClass(StateStruct _state, TextMeshProUGUI _text)
        {
            state = _state;
            textMesh = _text;
        }
    }

    class ScoreClass
    {
        public ScoreClass(int _count, List<CardScript> _list, TextMeshProUGUI _text)
        {
            count = _count;
            list = _list;
            text = _text;
        }
        public int count;
        public int point = 0;
        public int max = 0;
        public int score = 0;
        public List<CardScript> list;
        public TextMeshProUGUI text;
    }

    // Start is called before the first frame update
    public int padding;
    public bool hasRain;

    public GameObject cardPrefabs;

    public Transform lightArea;
    public Transform bandArea;
    public Transform specialArea;
    public Transform normalArea; 
    
    public Transform otherLightArea;
    public Transform otherBandArea;
    public Transform otherSpecialArea;
    public Transform otherNormalArea;

    public GameObject myTexts;
    public TextMeshProUGUI myScore;
    public TextMeshProUGUI myState;
        
    public GameObject otherTexts;
    public TextMeshProUGUI otherScore;
    public TextMeshProUGUI otherState;

    //Dictionary<CARD_TYPE, int> cardCount;
    Dictionary<CARD_TYPE, List<CardScript>> cardLists;
    Dictionary<CARD_TYPE, List<CardScript>> otherCardLists;
    Dictionary<CARD_TYPE, ScoreClass> pointDict;
    Dictionary<CARD_TYPE, ScoreClass> otherPointDict;

    Dictionary<bool, Dictionary<SPECIAL_CARD, int>> specialDict;
    
    Dictionary<ScoreState, StateClass> stateDict;
    Dictionary<ScoreState, StateClass> otherStateDict;


    Dictionary<bool, int> scoreDict = new Dictionary<bool, int>();
    Dictionary<bool, TextMeshProUGUI> stateTextDict = new Dictionary<bool, TextMeshProUGUI>();

    Queue<Coroutine> coroutineQueue = new Queue<Coroutine>();

    protected override void OnStart()
    {
        //cardCount = new Dictionary<CARD_TYPE, int>();
        cardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        pointDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherPointDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherCardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        stateDict = new Dictionary<ScoreState, StateClass>();
        otherStateDict = new Dictionary<ScoreState, StateClass>();
        specialDict = new Dictionary<bool, Dictionary<SPECIAL_CARD, int>>();

        
        stateTextDict.Add(true, myState);
        stateTextDict.Add(false, otherState);
        for (int i = 0; i <2; i++)
        {
            bool isMine = i % 2 == 0;
            scoreDict.Add(isMine, 0);
            specialDict.Add(isMine, new Dictionary<SPECIAL_CARD, int>());
            stateTextDict[isMine].text = "";
            foreach(SPECIAL_CARD sp in Enum.GetValues(typeof(SPECIAL_CARD)))
            {
                specialDict[isMine].Add(sp, 0);
            }
        }
        myScore.text = scoreDict[true].ToString();
        otherScore.text = scoreDict[false].ToString();





        string[] strings = { "고", "흔", "뻑"};
        TextMeshProUGUI[] my = myTexts.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] other = otherTexts.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i <3; i++)
        {   
            ScoreState scoreState = (ScoreState)i;
            StateStruct stateStruct = new StateStruct(scoreState, 0, strings[i]);
            
            stateDict.Add(scoreState, new StateClass(stateStruct, my[i]));
            otherStateDict.Add(scoreState, new StateClass(stateStruct, other[i]));
        }

        Initiate(10, CARD_TYPE.BAND, bandArea);
        Initiate(10, CARD_TYPE.SPECIAL, specialArea);
        Initiate(5, CARD_TYPE.LIGHT, lightArea);
        Initiate(25, CARD_TYPE.NORMAL, normalArea);

        Initiate(10, CARD_TYPE.BAND, otherBandArea, false);
        Initiate(10, CARD_TYPE.SPECIAL, otherSpecialArea, false);
        Initiate(5, CARD_TYPE.LIGHT, otherLightArea, false);
        Initiate(25, CARD_TYPE.NORMAL, otherNormalArea, false);

        EventManager.AddListener(Constants.FINISH_GAME, FinishGame);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
    }
    void Initiate(int count, CARD_TYPE type, Transform parent, bool isMine = true)
    {
        List<CardScript>  list = new List<CardScript>(count);
        TextMeshProUGUI text = parent.GetComponentInChildren<TextMeshProUGUI>();
        InitList(count, parent, list);
        text.transform.SetAsLastSibling();
        ScoreClass scoreClass = new ScoreClass(0, list, text);
        if (isMine)
        {
            cardLists.Add(type, list);
            pointDict.Add(type, scoreClass);
        }
        else
        {
            otherCardLists.Add(type, list);
            otherPointDict.Add(type, scoreClass);
        }
        SetText(type, isMine);
    }
    void InitList(int count, Transform parent, List<CardScript> list){
        for(int i = 0 ; i< count;i++){
            GameObject go = Instantiate(cardPrefabs, parent);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            list.Add(go.GetComponent<CardScript>());
        }
    }
    public CardClass PassCard()
    {
        if (pointDict[CARD_TYPE.NORMAL].count > 0)
        {
            CardClass card = null;
            for(int i = 0; i < pointDict[CARD_TYPE.NORMAL].count; i++)
            {
                if(cardLists[CARD_TYPE.NORMAL][i].GetCardClass().score == 1)
                {
                    card = cardLists[CARD_TYPE.NORMAL][i].RemoveCard();
                    for(int j = i+1; j < pointDict[CARD_TYPE.NORMAL].count; j++)
                    {
                        CardClass temp = cardLists[CARD_TYPE.NORMAL][j].RemoveCard();
                        cardLists[CARD_TYPE.NORMAL][j - 1].SetCard(temp);
                    }
                }
                if (card != null)
                {
                    pointDict[CARD_TYPE.NORMAL].count--;
                    pointDict[CARD_TYPE.NORMAL].point -= card.score;
                    SetText(CARD_TYPE.NORMAL, true);
                    return card;
                }
            }
            card = cardLists[CARD_TYPE.NORMAL][0].RemoveCard();
            for(int i = 1; i < pointDict[CARD_TYPE.NORMAL].count; i++)
            {
                CardClass temp = cardLists[CARD_TYPE.NORMAL][i].RemoveCard();
                cardLists[CARD_TYPE.NORMAL][i - 1].SetCard(temp);
            }
            pointDict[CARD_TYPE.NORMAL].count--;
            pointDict[CARD_TYPE.NORMAL].point -= card.score;
            SetText(CARD_TYPE.NORMAL, true);
            return card;
        }
        return null;
    }
    public void AddPoint(CardClass cardClass, bool isMine = true)
    {
        CARD_TYPE type = cardClass.type;
        if (isMine)
        {
            cardLists[type][pointDict[cardClass.type].count++].SetCard(cardClass);
            pointDict[type].point += cardClass.score;
            if(pointDict[type].point > pointDict[type].max)
            {
                pointDict[type].max = pointDict[type].point;
            }
            
        }
        else
        {
            otherCardLists[cardClass.type][otherPointDict[cardClass.type].count++].SetCard(cardClass);
            otherPointDict[cardClass.type].point += cardClass.score;
            if(otherPointDict[type].point > otherPointDict[type].max)
            {
                otherPointDict[type].max = otherPointDict[type].point;
            }
        }
        specialDict[isMine][cardClass.sCard]++;
        hasRain = CheckLight(cardClass);
        CalculateScore(cardClass.type, isMine);
        SetText(cardClass.type, isMine);
        KeyValuePair<SPECIAL_CARD, int> pair = CheckBand(cardClass, isMine);
        AddSpecial(pair, isMine);
        if (isMine) myScore.text = scoreDict[isMine].ToString();
        else otherScore.text = scoreDict[isMine].ToString();

    }
    void AddSpecial(KeyValuePair<SPECIAL_CARD, int> pair, bool isMine)
    {
        string stateString = "";
        const string warning = "비상";
        switch (pair.Value)
        {
            case 2:
                switch (pair.Key)
                {
                    case SPECIAL_CARD.FIVEVBIRD:
                        stateString = $"고도리 {warning}";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        Debug.Log(stateString);
                        break;
                    case SPECIAL_CARD.BLUEBAND:
                        stateString = $"청단 {warning}";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        Debug.Log(stateString);
                        break;
                    case SPECIAL_CARD.REDBAND:
                        stateString = $"홍단 {warning}";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        Debug.Log(stateString);
                        break;
                    case SPECIAL_CARD.GRASSBAND:
                        stateString = $"초단 {warning}";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        Debug.Log(stateString);
                        break;
                }
                break;
            case 3:
                switch (pair.Key)
                {
                    case SPECIAL_CARD.FIVEVBIRD:
                        stateString = "고도리";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        Debug.Log(stateString);
                        scoreDict[isMine] += 5;
                        break;
                    case SPECIAL_CARD.BLUEBAND:
                        stateString = "청단";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        scoreDict[isMine] += 3;
                        Debug.Log(stateString);
                        break;
                    case SPECIAL_CARD.REDBAND:
                        stateString = "홍단";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        scoreDict[isMine] += 3;
                        Debug.Log(stateString);
                        break;
                    case SPECIAL_CARD.GRASSBAND:
                        stateString = "초단";
                        StartCoroutine(SetStateText(stateTextDict[isMine], stateString));
                        scoreDict[isMine] += 2;
                        Debug.Log(stateString);
                        break;
                }
                break;
        }
    }
    void CalculateScore(CARD_TYPE type, bool isMine)
    {
        if (isMine)
        {
            int preScore = 0;
            switch (type)
            {
                case CARD_TYPE.NORMAL:
                    if (pointDict[type].max >= 10)
                    {
                        preScore = pointDict[type].score;
                        pointDict[type].score = pointDict[type].max - 9;
                        int sub = pointDict[type].score - preScore;
                        scoreDict[isMine] += sub;
                    }
                    break;
                case CARD_TYPE.BAND:
                case CARD_TYPE.SPECIAL:
                    if (pointDict[type].max >= 5)
                    {
                        preScore = pointDict[type].score;
                        pointDict[type].score = pointDict[type].max - 4;
                        int sub = pointDict[type].score - preScore;
                        scoreDict[isMine] += sub;
                    }
                    break;
                case CARD_TYPE.LIGHT:
                    string lightString = "";
                    switch (pointDict[type].max) {
                        case 3:
                            //비 삼광인지 아닌지
                            if (hasRain)
                            {
                                scoreDict[isMine] += 2;
                                lightString = "비삼광";
                                coroutineQueue.Enqueue(StartCoroutine(SetStateText(stateTextDict[isMine], lightString)));
                                Debug.Log(lightString);
                            }
                            else
                            {
                                scoreDict[isMine] += 3;
                                lightString = "삼광";
                                coroutineQueue.Enqueue(StartCoroutine(SetStateText(stateTextDict[isMine], lightString)));
                                Debug.Log(lightString);
                            }
                            break;
                        case 4:
                            scoreDict[isMine] += 4;
                            lightString = "사광";
                            coroutineQueue.Enqueue(StartCoroutine(SetStateText(stateTextDict[isMine], lightString)));
                            Debug.Log(lightString);
                            break;
                        case 5:
                            scoreDict[isMine] += 15;
                            lightString = "오광";
                            coroutineQueue.Enqueue(StartCoroutine(SetStateText(stateTextDict[isMine], lightString)));
                            Debug.Log(lightString);
                            break;
                    }
                    break;
            }
        }
        else
        {

        }
    }
    bool CheckLight(CardClass cardClass)
    {
        if (hasRain) return true;
        if(cardClass.month == 11)
        {
            return true;
        }
        return false;
    }
    KeyValuePair<SPECIAL_CARD, int> CheckBand(CardClass cardClass, bool isMine)
    {
        bool isPossible = true;
        SPECIAL_CARD sc = cardClass.sCard;
        if (sc == SPECIAL_CARD.NORMAL) return new KeyValuePair<SPECIAL_CARD, int>(sc, 0);
        //내가 한 장 이상 가지고 있음.
        if (specialDict[isMine][sc] > 0)
        {
            // 상대방도 가지고 있음. : 불가능.
            if (specialDict[!isMine][sc] > 0) isPossible = false;
        }
        // 불가능이면 0 리턴.
        if (!isPossible) return new KeyValuePair<SPECIAL_CARD, int>(sc, 1);
        else
        {
            // 가능한 경우에,
            switch (specialDict[isMine][sc])
            {

                case 2:
                    // 2장 있음 -> 비상, 경고 등 표시
                    return new KeyValuePair<SPECIAL_CARD, int>(sc, 2);
                case 3:
                    // 3장 있음 -> 달성했음. 
                    return new KeyValuePair<SPECIAL_CARD, int>(sc, 3);
            }
        }
        // 점수랑 현재 상관 없음. 가능성은 열려있음
        return new KeyValuePair<SPECIAL_CARD, int>(sc, 0);
    }
    public void SetState(ScoreState state, bool isMine)
    {
        switch (isMine)
        {
            case true:
                stateDict[state].state.AddPoint();
                stateDict[state].textMesh.text = stateDict[state].state.point + $" {stateDict[state].state.name}";
                break;
            case false:
                otherStateDict[state].state.AddPoint();
                otherStateDict[state].textMesh.text = otherStateDict[state].state.point +" "+ otherStateDict[state].state.name;
                break;
        }
    }
    void SetText(CARD_TYPE type, bool isMine = true)    
    {
        if (isMine)
        {
            pointDict[type].text.text = pointDict[type].point > 0 ? pointDict[type].point.ToString() : "";
        }
        else
        {
            otherPointDict[type].text.text = otherPointDict[type].point > 0 ? otherPointDict[type].point.ToString() : "";
        }
    }
    
    void FinishGame(params object[] param)
    {
        hasRain = false;
        for(int i = 0; i < 2; i++)
        {
            bool isMine = i % 2 == 0;
            scoreDict[isMine] = 0;
            foreach(SPECIAL_CARD sc in Enum.GetValues(typeof(SPECIAL_CARD)))
            {
                specialDict[isMine][sc] = 0;
            }
        }
        myScore.text = scoreDict[true].ToString();
        otherScore.text = scoreDict[false].ToString();

        foreach (ScoreState state in Enum.GetValues(typeof(ScoreState)))
        {
            stateDict[state].state.point = 0;
            stateDict[state].textMesh.text = $"0 {stateDict[state].state.name}";
            
        }
        foreach (CARD_TYPE ct in Enum.GetValues(typeof(CARD_TYPE)))
        {

            pointDict[ct].point = 0;
            pointDict[ct].text.text = "";
            pointDict[ct].count = 0;
            pointDict[ct].score = 0;
            pointDict[ct].max = 0;
            for(int j = 0; j< pointDict[ct].list.Count; j++)
            {
                if (pointDict[ct].list[j].gameObject.activeInHierarchy)
                {
                    pointDict[ct].list[j].RemoveCard();
                }
                else break;
            }

            otherPointDict[ct].point = 0;
            otherPointDict[ct].text.text = "";
            otherPointDict[ct].count = 0;
            otherPointDict[ct].max = 0;
            otherPointDict[ct].score = 0;
            for (int j = 0; j < otherPointDict[ct].list.Count; j++)
            {
                if (otherPointDict[ct].list[j].gameObject.activeInHierarchy)
                {
                    otherPointDict[ct].list[j].RemoveCard();
                }
                else break;
            }

        }
        
    }
    public void SetStateTextMethod(string str)
    {
        StartCoroutine(SetStateText(stateTextDict[true], str));
    }
    IEnumerator SetStateText(TextMeshProUGUI tmp, string str)
    {
        tmp.text = str;
        yield return new WaitForSeconds(1.5f);
        tmp.text = "";
    }
    IEnumerator StartCoroutines()
    {
        while (coroutineQueue.Count > 0)
        {
            yield return coroutineQueue.Dequeue();
        }
    }
}
