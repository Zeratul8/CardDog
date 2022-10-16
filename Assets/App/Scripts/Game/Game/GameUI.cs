using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    const string START_GAME = "StartGame";
    List<int> intList;
    List<CardClass> basicDeck;
    List<int> SPArr = new List<int>(){4, 12, 16, 20, 24, 29, 32, 36, 41};
    List<int> Light = new List<int>(){0, 8, 28, 40, 44};
    List<int> Blue = new List<int>(){21, 33, 37};
    List<int> Red = new List<int>(){1, 5, 9};
    List<int> Grass = new List<int>(){13, 17, 25};
    List<int> Bird = new List<int>(){4, 12, 29};
    public List<Sprite> sprites;
    private void Awake() {
        intList = new List<int>();
        for(int i = 0; i<50;i++){
            intList.Add(i);
        }
        
        basicDeck = new List<CardClass>();
        SetDeck();
    }
    void SetDeck(){
        for(int i = 0 ; i<50;i++){
            CARD_TYPE cType = CARD_TYPE.NORMAL;
            SPECIAL_CARD sCard = SPECIAL_CARD.NORMAL;
            if(SPArr.Contains(i)){
                cType = CARD_TYPE.SPECIAL;
            }
            else if (Light.Contains(i)){
                cType = CARD_TYPE.LIGHT;
            }
            else if (i == 46) cType = CARD_TYPE.BAND;

            if (Blue.Contains(i)){
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.BLUEBAND;
            }
            else if (Red.Contains(i)){
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.REDBAND;
            }
            else if (Grass.Contains(i)){
                cType = CARD_TYPE.BAND;
                sCard = SPECIAL_CARD.GRASSBAND;
            }            
            else if (Bird.Contains(i)){
                sCard = SPECIAL_CARD.FIVEVBIRD;
            }
            if(i/4 < 12){
                CardClass c = new CardClass(){
                    index = i,
                    month = i/4,
                    score = i==41 || i == 47 ? 2 : 1,
                    type = cType,
                    sCard = sCard,
                };
                basicDeck.Add(c);
            }else{
                CardClass c = new CardClass(){
                    index = i,
                    month = -1,
                    score = 2 + i%48,
                    type = CARD_TYPE.JOKER,
                    sCard = sCard,
                };
                basicDeck.Add(c);
            }
        }
    }
    public void ClickStart(){
        Debug.Log("Start");
        StartCoroutine(ShuffleCoroutine());
    }
    IEnumerator ShuffleCoroutine(){
        Shuffle();
        yield return new WaitForSeconds(2);
        EventManager.CallEvent(START_GAME, Shuffle());
    }
    List<CardClass> Shuffle(){
        List<CardClass> deck = new List<CardClass>();

        for(int i = 0 ; i< intList.Count;i++){
            int r = Random.Range(0, intList.Count);
            int j = intList[i];
            intList[i] = intList[r];
            intList[r] = j;
        }

        for(int i = 0 ; i< intList.Count; i++){
            deck.Add(basicDeck[intList[i]]);
        }
        return deck;
    }
}
