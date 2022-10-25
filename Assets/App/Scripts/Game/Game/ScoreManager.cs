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

        for(int i = 0 ; i< 5;i++){
            GameObject go = Instantiate(cardPrefabs, lightArea);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            lightList.Add(go.GetComponent<CardScript>());
        }
        for(int i = 0 ; i <10; i++){
            GameObject go = Instantiate(cardPrefabs, bandArea);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            bandList.Add(go.GetComponent<CardScript>());
        }
        for(int i = 0 ; i<10; i++){
            GameObject go = Instantiate(cardPrefabs, specialArea);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            specialList.Add(go.GetComponent<CardScript>());
        }
        for(int i = 0 ; i <20 ; i++){
            GameObject go = Instantiate(cardPrefabs, normalArea);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(i*padding, 0);
            normalList.Add(go.GetComponent<CardScript>());
        }
    }
    public void AddPoint(CardClass cardClass)
    {
        cardLists[cardClass.type][cardCount[cardClass.type]++].SetScore(cardClass);
        switch (cardClass.type)
        {
            case CARD_TYPE.BAND:
                break;
            case CARD_TYPE.LIGHT:
                break;
            case CARD_TYPE.SPECIAL:
                break;
            case CARD_TYPE.NORMAL:
                break;
            case CARD_TYPE.JOKER:
                break;
        }
    }
}
