using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    // Start is called before the first frame update
    public int padding;

    public GameObject cardPrefabs;

    public Transform bandArea;
    public Transform specialArea;
    public Transform lightArea;
    public Transform normalArea;
    

    List<CardScript> bandList;
    List<CardScript> specialList;
    List<CardScript> lightList;
    List<CardScript> normalList;

    Dictionary<CARD_TYPE, int> cardCount;
    Dictionary<CARD_TYPE, List<CardScript>> cardLists;
    protected override void OnStart()
    {
        cardCount = new Dictionary<CARD_TYPE, int>();
        cardLists = new Dictionary<CARD_TYPE, List<CardScript>>();

        bandList = new List<CardScript>();
        specialList = new List<CardScript>();
        lightList = new List<CardScript>();
        normalList = new List<CardScript>();

        cardLists.Add(CARD_TYPE.NORMAL, normalList);
        cardLists.Add(CARD_TYPE.LIGHT, lightList);
        cardLists.Add(CARD_TYPE.BAND, bandList);
        cardLists.Add(CARD_TYPE.SPECIAL, specialList);

        cardCount.Add(CARD_TYPE.NORMAL , 0);
        cardCount.Add(CARD_TYPE.LIGHT , 0);
        cardCount.Add(CARD_TYPE.BAND, 0);
        cardCount.Add(CARD_TYPE.SPECIAL , 0);

        InitList(5, lightArea, lightList);
        InitList(10, bandArea, bandList);
        InitList(10, specialArea, specialList);
        InitList(20, normalArea, normalList);

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
        cardLists[cardClass.type][cardCount[cardClass.type]++].SetScore(cardClass);
        
    }
}
