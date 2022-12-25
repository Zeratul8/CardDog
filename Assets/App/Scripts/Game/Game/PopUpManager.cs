using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : SingletonMonoBehaviour<PopUpManager>
{
    [SerializeField]
    private GameObject selectObject;
    [SerializeField]
    GameUI gameUI;
    Dictionary<Button, Image> selectButtons;
    int selectIndex = -1;
    
    protected override void OnAwake()
    {
        base.OnAwake();
    }
    protected override void OnStart()
    {
        base.OnStart();
        Button[] selectButton;
        selectButton = selectObject.GetComponentsInChildren<Button>();
        for(int i = 0; i < selectButton.Length; i++)
        {
            selectButtons.Add(selectButton[i], selectButton[i].GetComponent<Image>());
            int index = i;
            selectButton[i].onClick.AddListener(() => { });
        }
    }

    public int SetSelectCard(List<CardScript> cards)
    {
        gameObject.SetActive(true);
        selectObject.SetActive(true);
        CardClass left = cards[0].GetCardClass();
        CardClass right = cards[1].GetCardClass();
        int index = 0;
        foreach(Image image in selectButtons.Values)
        {
            image.sprite = gameUI.sprites[cards[index].GetCardClass().index];
            index++;
        }
        return 0;
    }
}
