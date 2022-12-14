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
    GameObject borderLine;
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
        EventManager.AddListener(Constants.READY_TO_PLAY, ReadyToPlay);
        if (transform.childCount > 0)
        {
            borderLine = transform.GetChild(0).gameObject;
            Debug.Log(borderLine.name);
            borderLine.SetActive(false);
        }
    }
    
    private void OnDestroy() {
        EventManager.RemoveListener(Constants.FINISH_GAME, FinishGame);
        EventManager.RemoveListener(Constants.READY_TO_PLAY, ReadyToPlay);
    }
    #endregion

    #region Public Methods
    public CardClass GetCardClass(){
        return myCardData;
    }
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
    public CardClass RemoveCard(){
        // Debug.Log(myCardData.month);
        // Debug.Log(myCardData.index);
        gameObject.SetActive(false);
        return myCardData;
    }
    public void SetCard(CardClass cardClass){
        gameObject.SetActive(true);
        image.sprite = gameUI.sprites[cardClass.index];
        myCardData = cardClass;
    }
    void OnClick()
    {
        if (PlayerManager.Instance.isMyTurn)
        {
            if (myCardData.index > 47)
            {
                //gameObject.SetActive(false);
                minusCard();    //?????????? ????????
                ScoreManager.Instance.AddPoint(myCardData);
                EventManager.CallEvent(Constants.POP_CARD);
                EventManager.CallEvent(Constants.PLUS_HAND, this);
                // PlayerManager.Instance.SortHand();
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
    void ReadyToPlay(params object[] param)
    {
        if (transform.childCount > 0)
            StartCoroutine(CheckBorderOn());
    }
    void FinishGame(params object[] param)
    {
        if(borderLine != null)borderLine.SetActive(false);
        gameObject.SetActive(false);
        StopAllCoroutines();
    }
    IEnumerator CheckBorderOn()
    {
        if (myCardData.month >= 0)
        {
            yield return new WaitUntil(() => PlayerManager.Instance.floorDict[myCardData.month]);
            borderLine.SetActive(true);
            StartCoroutine(CheckBorderOff());
        }

    }
    IEnumerator CheckBorderOff()
    {
        if (myCardData.month >= 0)
        {
            yield return new WaitWhile(() => PlayerManager.Instance.floorDict[myCardData.month]);
            borderLine.SetActive(false);
            StartCoroutine(CheckBorderOn());
        }
    }
    #endregion
}
