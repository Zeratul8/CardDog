using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    const string START_GAME = "StartGame";
    public CardScript[] cards;
    private void Awake() {
        // cards = FindObjectsOfType<CardScript>();
        EventManager.AddListner(START_GAME, (object[] param) =>{
            List<CardClass> list = (List<CardClass>)param[0];
            StartCoroutine(ShareCard(list));
        });
    }
    IEnumerator ShareCard(List<CardClass> list){
        for(int i = 0 ; i<5;i++){
            cards[i].InitCard(list[i]);
            yield return null;
        }
        yield return new WaitForSeconds(2);
        for(int i = 5 ; i<10; i++){
            cards[i].InitCard(list[i]);
            yield return null;
        }
    }
    
}
