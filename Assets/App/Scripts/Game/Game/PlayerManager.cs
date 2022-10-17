using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CardScript[] cards;
    int myHand = 0;
    private void Awake()
    {
        EventManager.AddListner(Constants.START_GAME, StartGame);
        EventManager.AddListner(Constants.ADD_HAND, AddHand);
        EventManager.AddListner(Constants.PUT_CARD, PutCard);
    }
    IEnumerator ShareCard(List<CardClass> list, bool isFirst)
    {
        const int floorCard = 4;
        const int shareCard = 5;
        int myCard = floorCard;
        if (!isFirst) myCard += shareCard;
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
        StartCoroutine(CheckCard());
    }
    void AddHand(object[] param){
        myHand++;
    }
    void PutCard(object[] param){
        myHand--;
    }
    void StartGame(object[] param){
        List<CardClass> list = (List<CardClass>)param[0];
            bool isFirst = (bool)param[1];
            StartCoroutine(ShareCard(list, isFirst));
    }
    IEnumerator CheckCard(){
        yield return new WaitWhile(() => myHand != 0);
        EventManager.CallEvent(Constants.FINISH_GAME);
    }
    private void OnDestroy() {
        EventManager.RemoveListener(Constants.START_GAME, StartGame);
        EventManager.RemoveListener(Constants.ADD_HAND, AddHand);
        EventManager.RemoveListener(Constants.PUT_CARD, PutCard);
    }
}
