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
        button.onClick.AddListener(OnClick);
        image = GetComponent<Image>();
        gameUI = GetComponentInParent<GameUI>();
    }
    public void InitCard(CardClass cardClass)
    {
        gameObject.SetActive(true);
        myCardData = cardClass;
        
        image.sprite = gameUI.sprites[cardClass.index];
    }
    void OnClick()
    {
        Debug.Log(myCardData.index.ToString());
        Debug.Log(myCardData.month.ToString());
        
        gameObject.SetActive(false);
    }

}
