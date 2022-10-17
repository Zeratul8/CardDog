using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    const string START_GAME = "StartGame";
    public CardScript[] cards;
    private void Awake() {
        EventManager.AddListner(START_GAME, (object[] param) =>{
            List<CardClass> list = (List<CardClass>)param[0];
            bool isFirst = (bool)param[1];
            StartCoroutine(ShareCard(list, isFirst));
        });
    }
    IEnumerator ShareCard(List<CardClass> list, bool isFirst)
    {
        const int floorCard = 4;
        const int shareCard = 5;
        int myCard = floorCard;
        if(!isFirst) myCard += shareCard;
        for (int i = 0; i < 5; i++)
            {
                Debug.Log(myCard);
                cards[i].InitCard(list[myCard++]);
                yield return null;
            }
            yield return new WaitForSeconds(2);
            myCard += floorCard + shareCard;
            for (int i = 5; i < 10; i++)
            {
                Debug.Log(myCard);
                cards[i].InitCard(list[myCard++]);
                yield return null;
            }
    }
    /*
    바닥 0~3
    선 4~8
    후 9~13
    바닥 14~17
    선 18~22
    후 23~27
    */

}
