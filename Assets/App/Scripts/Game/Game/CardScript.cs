using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    Button button = null;
    CardClass myCardData;
    Image image;
    GameUI gameUI;
    private void Awake()
    {
        gameObject.SetActive(false);
        button = GetComponent<Button>();
        if(button!=null)button.onClick.AddListener(OnClick);
        image = GetComponent<Image>();
        gameUI = GetComponentInParent<GameUI>();
    }
    public void SetHand(CardClass cardClass, bool isMine)
    {
        gameObject.SetActive(true);
        if (isMine)
        {
            image.sprite = gameUI.sprites[cardClass.index];
            EventManager.CallEvent(Constants.PLUS_HAND);
            myCardData = cardClass;
        }
    }
    public void SetFloor(CardClass cardClass){
        gameObject.SetActive(true);
        image.sprite = gameUI.sprites[cardClass.index];
        myCardData = cardClass;
    }
    void OnClick()
    {
        Debug.Log(myCardData.index.ToString());
        Debug.Log(myCardData.month.ToString());
        
        gameObject.SetActive(false);
        minusCard();
        EventManager.CallEvent(Constants.SET_FLOOR_CARD, myCardData, true);
    }
    void minusCard()
    {
        EventManager.CallEvent(Constants.MINUS_CARD);
    }
}
