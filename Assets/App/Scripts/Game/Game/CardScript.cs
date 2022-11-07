using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    #region Constants and Fields
    Button button = null;
    CardClass myCardData;
    Image image;
    GameUI gameUI;
    #endregion

    #region Properties
    #endregion

    #region Unity Methods
     private void Awake()
    {
        gameObject.SetActive(false);
        button = GetComponent<Button>();
        if(button!=null)button.onClick.AddListener(OnClick);
        image = GetComponent<Image>();
        gameUI = GetComponentInParent<GameUI>();
        EventManager.AddListener(Constants.FINISH_GAME, FinishGame);
    }
    private void OnDestroy() {
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
    }
    #endregion

    #region Public Methods
     
    public void SetFloor(CardClass cardClass){
        gameObject.SetActive(true);
        image.sprite = gameUI.sprites[cardClass.index];
        myCardData = cardClass;
    }
    #endregion

    #region Methods
    public void SetHand(CardClass cardClass, bool isMine)
    {
        gameObject.SetActive(true);
        if (isMine)
        {
            image.sprite = gameUI.sprites[cardClass.index];
            myCardData = cardClass;
        }
    }
    public void SetHand(CardClass cardClass){
        if(gameObject.activeInHierarchy){
            image.sprite = gameUI.sprites[cardClass.index];
            myCardData = cardClass;
        }
    }
    public CardClass RemoveCard(){
        // Debug.Log(myCardData.month);
        // Debug.Log(myCardData.index);
        gameObject.SetActive(false);
        return myCardData;
    }
    public void SetScore(CardClass cardClass){
        gameObject.SetActive(true);
        image.sprite = gameUI.sprites[cardClass.index];
        myCardData = cardClass;
    }
    void OnClick()
    {
        if (PlayerManager.Instance.isMyTurn)
        {
            Debug.Log(myCardData.index);
            if (myCardData.index > 47)
            {
                //gameObject.SetActive(false);
                minusCard();    //점수판으로 보내야함
                EventManager.CallEvent(Constants.POP_CARD);
                EventManager.CallEvent(Constants.PLUS_HAND);
                PlayerManager.Instance.SortHand();
            }
            else
            {
                gameObject.SetActive(false);
                minusCard();
                EventManager.CallEvent(Constants.SET_FLOOR_CARD, myCardData, true);
            }
        }
    }
    void minusCard()
    {
        EventManager.CallEvent(Constants.MINUS_HAND);
    }
    void FinishGame(params object[] param){
        gameObject.SetActive(false);
    }
    #endregion
}
