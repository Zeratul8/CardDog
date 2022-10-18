using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public CardScript[] cards;
    int myHand = 0;
    WaitForSeconds waitSeconds = new WaitForSeconds(0.2f);
    private void Awake()
    {
        EventManager.AddListner(Constants.START_GAME, StartGame);
        EventManager.AddListner(Constants.ADD_HAND, AddHand);
        EventManager.AddListner(Constants.PUT_CARD, PutCard);
    }
    IEnumerator ShareCard(List<CardClass> list, bool isFirst)
    {
        //카드 깔기.
        const int floorCard = 4;
        const int shareCard = 5;
        int myCard = floorCard;
            //바닥에 패깔기 반복문
        for(int i = 0; i < floorCard; i++){
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
        }
        if (!isFirst) myCard += shareCard;
        //내 카드 받기
        for (int i = 0; i < 5; i++)
        {
            Debug.Log(myCard);
            cards[i].InitCard(list[myCard++]);
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
        }
        //상대 카드 주기
        for(int i = 0 ; i<shareCard;i++){
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
        }
            //바닥에 패깔기 반복문
        for(int i = 0; i < floorCard; i++){
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
        }
        myCard += floorCard + shareCard;
        //상대 카드 주기
        for(int i = 0 ; i<shareCard;i++){   
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
        }
        //내 카드 받기
        for (int i = 5; i < 10; i++)
        {
            Debug.Log(myCard);
            cards[i].InitCard(list[myCard++]);
            EventManager.CallEvent("PopCard");
            yield return waitSeconds;
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
