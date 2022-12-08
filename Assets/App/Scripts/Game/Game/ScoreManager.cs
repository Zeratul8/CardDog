using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ScoreState
{
    Go,
    Shake,
    Poop
}
public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    
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
    Dictionary<CARD_TYPE, ScoreClass> scoreDict;
    Dictionary<CARD_TYPE, ScoreClass> otherScoreDict;
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
    Dictionary<ScoreState, StateClass> stateDict;
    Dictionary<ScoreState, StateClass> otherStateDict;

    public GameObject myTexts;
    public GameObject otherTexts;

    struct StateStruct
    {
        public ScoreState state { get; }
        public string name { get; }
        public int point;
        public void AddPoint()
        {
            point++;
            Debug.Log(point);
        }

            public StateStruct(ScoreState _state, int _point, string _name)
        {
            state = _state;
            name = _name;
            point = _point;
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
        public int score = 0;
        public List<CardScript> list;
        public TextMeshProUGUI text;
    }
    protected override void OnStart()
    {
        //cardCount = new Dictionary<CARD_TYPE, int>();
        cardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        scoreDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherScoreDict = new Dictionary<CARD_TYPE, ScoreClass>();
        otherCardLists = new Dictionary<CARD_TYPE, List<CardScript>>();
        stateDict = new Dictionary<ScoreState, StateClass>();
        otherStateDict = new Dictionary<ScoreState, StateClass>();
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
            scoreDict.Add(type, scoreClass);
        }
        else
        {
            otherCardLists.Add(type, list);
            otherScoreDict.Add(type, scoreClass);
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
        if (scoreDict[CARD_TYPE.NORMAL].count > 0)
        {
            CardClass card = null;
            for(int i = 0; i < scoreDict[CARD_TYPE.NORMAL].count; i++)
            {
                if(cardLists[CARD_TYPE.NORMAL][i].GetCardClass().score == 1)
                {
                    card = cardLists[CARD_TYPE.NORMAL][i].RemoveCard();
                    for(int j = i+1; j < scoreDict[CARD_TYPE.NORMAL].count; j++)
                    {
                        CardClass temp = cardLists[CARD_TYPE.NORMAL][j].RemoveCard();
                        cardLists[CARD_TYPE.NORMAL][j - 1].SetCard(temp);
                    }
                }
                if (card != null)
                {
                    scoreDict[CARD_TYPE.NORMAL].count--;
                    scoreDict[CARD_TYPE.NORMAL].score -= card.score;
                    SetText(CARD_TYPE.NORMAL, true);
                    return card;
                }
            }
            card = cardLists[CARD_TYPE.NORMAL][0].RemoveCard();
            for(int i = 1; i < scoreDict[CARD_TYPE.NORMAL].count; i++)
            {
                CardClass temp = cardLists[CARD_TYPE.NORMAL][i].RemoveCard();
                cardLists[CARD_TYPE.NORMAL][i - 1].SetCard(temp);
            }
            scoreDict[CARD_TYPE.NORMAL].count--;
            scoreDict[CARD_TYPE.NORMAL].score -= card.score;
            SetText(CARD_TYPE.NORMAL, true);
            return card;
        }
        return null;
    }
    public void AddPoint(CardClass cardClass, bool isMine = true)
    {
        if (isMine)
        {
            if (cardClass.type == CARD_TYPE.NORMAL)
            {
                cardLists[cardClass.type][scoreDict[cardClass.type].count++].SetCard(cardClass);
                scoreDict[cardClass.type].score += cardClass.score;
            }
            else
            {
                cardLists[cardClass.type][scoreDict[cardClass.type].count++].SetCard(cardClass);
                scoreDict[cardClass.type].score = scoreDict[cardClass.type].count;
            }
        }
        else
        {
            if (cardClass.type == CARD_TYPE.NORMAL)
            {
                otherCardLists[cardClass.type][otherScoreDict[cardClass.type].count++].SetCard(cardClass);
                otherScoreDict[cardClass.type].score += cardClass.score;
            }
            else
            {
                otherCardLists[cardClass.type][otherScoreDict[cardClass.type].count++].SetCard(cardClass);
                otherScoreDict[cardClass.type].score = otherScoreDict[cardClass.type].count;
            }
        }
        
        SetText(cardClass.type, isMine);
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
            scoreDict[type].text.text = scoreDict[type].score > 0 ? scoreDict[type].score.ToString() : "";
        }
        else
        {
            otherScoreDict[type].text.text = otherScoreDict[type].score > 0 ? otherScoreDict[type].score.ToString() : "";
        }
    }
    
    void FinishGame(params object[] param)
    {
        for(int i = 0; i <3; i++)
        {
            //stateDict[(ScoreState)i] = 0;
        }
        for(int i = 0; i < 4; i++)
        {
            CARD_TYPE ct = (CARD_TYPE)i;
            scoreDict[ct].score = 0;
            scoreDict[ct].text.text = "";
            scoreDict[ct].count = 0;
            for(int j = 0; j< scoreDict[ct].list.Count; j++)
            {
                if (scoreDict[ct].list[j].gameObject.activeInHierarchy)
                {
                    scoreDict[ct].list[j].RemoveCard();
                }
                else break;
            }

            otherScoreDict[ct].score = 0;
            otherScoreDict[ct].text.text = "";
            otherScoreDict[ct].count = 0;
            for (int j = 0; j < otherScoreDict[ct].list.Count; j++)
            {
                if (otherScoreDict[ct].list[j].gameObject.activeInHierarchy)
                {
                    otherScoreDict[ct].list[j].RemoveCard();
                }
                else break;
            }

        }
        
    }
}
