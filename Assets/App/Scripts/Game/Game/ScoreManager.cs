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

    public GameObject cardPrefabs;

    public Transform lightArea;
    public Transform bandArea;
    public Transform specialArea;
    public Transform normalArea; 
    
    public Transform otherLightArea;
    public Transform otherBandArea;
    public Transform otherSpecialArea;
    public Transform otherNormalArea;

    //Dictionary<CARD_TYPE, int> cardCount;
    Dictionary<CARD_TYPE, List<CardScript>> cardLists;
    Dictionary<CARD_TYPE, List<CardScript>> otherCardLists;
    Dictionary<CARD_TYPE, ScoreClass> pointDict;
    Dictionary<CARD_TYPE, ScoreClass> otherPointDict;

    Dictionary<bool, Dictionary<SPECIAL_CARD, int>> specialDict;
    
    Dictionary<ScoreState, StateClass> stateDict;
    Dictionary<ScoreState, StateClass> otherStateDict;

    public GameObject myTexts;
    public GameObject otherTexts;

    Dictionary<bool, int> scoreDict = new Dictionary<bool, int>();

    protected override void OnStart()
    {
        //cardCount = new Dictionary<CARD_TYPE, int>();
        cardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        pointDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherPointDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherCardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        stateDict = new Dictionary<ScoreState, StateClass>();
        otherStateDict = new Dictionary<ScoreState, StateClass>();
        
        for(int i = 0; i <2; i++)
        {
            bool isMine = i % 2 == 0;
            scoreDict.Add(isMine, 0);
            
        }

        

        string[] strings = { "°í", "Èç", "»¶"};
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
        CalculateScore(cardClass.type, isMine);
        SetText(cardClass.type, isMine);
    }
    void CalculateScore(CARD_TYPE type, bool isMine)
    {
        switch (type)
        {
            case CARD_TYPE.NORMAL:
                if (pointDict[type].max >= 10)
                {
                    pointDict[type].score = pointDict[type].max - 9;
                }
                break;
            case CARD_TYPE.BAND:
            case CARD_TYPE.SPECIAL:
                if(pointDict[type].max >= 5)
                {
                    pointDict[type].score = pointDict[type].max - 5;
                }
                break;
            case CARD_TYPE.LIGHT:
                break;
        }
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
        foreach(ScoreState state in Enum.GetValues(typeof(ScoreState)))
        {
            stateDict[state].state.point = 0;
        }
        foreach (CARD_TYPE ct in Enum.GetValues(typeof(CARD_TYPE)))
        {

            pointDict[ct].point = 0;
            pointDict[ct].text.text = "";
            pointDict[ct].count = 0;
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
}
