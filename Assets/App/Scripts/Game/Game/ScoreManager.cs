using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    // Start is called before the first frame update
    public int padding;

    public GameObject cardPrefabs;

    public Transform bandArea;
    public Transform specialArea;
    public Transform lightArea;
    public Transform normalArea;

    //Dictionary<CARD_TYPE, int> cardCount;
    Dictionary<CARD_TYPE, List<CardScript>> cardLists;
    Dictionary<CARD_TYPE, ScoreClass> scoreDict;

    ScoreClass band;   
    ScoreClass special;   
    ScoreClass lightCard;   
    ScoreClass normal;   

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

        Initiate(10, CARD_TYPE.BAND, bandArea, band);
        Initiate(10, CARD_TYPE.SPECIAL, specialArea, special);
        Initiate(5, CARD_TYPE.LIGHT, lightArea, lightCard);
        Initiate(25, CARD_TYPE.NORMAL, normalArea, normal);
        EventManager.AddListener(Constants.FINISH_GAME, FinishGame);
    }
    private void OnDestroy()
    {
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
    }
    void Initiate(int count, CARD_TYPE type, Transform parent, ScoreClass scoreClass)
    {
        List<CardScript>  list = new List<CardScript>(count);
        TextMeshProUGUI text = parent.GetComponentInChildren<TextMeshProUGUI>();
        InitList(count, parent, list);
        text.transform.SetAsLastSibling();
        scoreClass = new ScoreClass(0, list, text);
        scoreDict.Add(type, scoreClass);
        cardLists.Add(type, list);
        SetText(type);
    }
    void InitList(int count, Transform parent, List<CardScript> list){
        for(int i = 0 ; i< count;i++){
            GameObject go = Instantiate(cardPrefabs, parent);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            list.Add(go.GetComponent<CardScript>());
        }
    }
    public void AddPoint(CardClass cardClass)
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
        SetText(cardClass.type);
    }
    void SetText(CARD_TYPE type)    
    {
        scoreDict[type].text.text = scoreDict[type].score > 0 ? scoreDict[type].score.ToString() : "" ;
    }
    void FinishGame(params object[] param)
    {
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

        }
        
    }
}
