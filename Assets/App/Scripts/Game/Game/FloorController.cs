using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    public Transform floorCardParent;
    public GameObject deckObject;
    List<GameObject> deck;
    int index = 0;
    // Start is called before the first frame update
    void Start()
    {
        deck = new List<GameObject>();
        EventManager.AddListner("PopCard", PopCard);
    }
    public void StartEvent(){
        if(deck.Count == 0){

        for(int i = 0 ; i <50;i++){
            GameObject go = Instantiate(deckObject, floorCardParent);
            deck.Add(go);
            float move = i*0.5f;
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(move, move);
                 index = i;
        }
        }
        else {
            for(int i = 0 ; i<deck.Count;i++){
                deck[i].SetActive(true);
                 index = i;
            }
        }
       
    }
    public void PopCard(object[] param){
        if(index>=0){
            deck[index].SetActive(false);
            index--;
        }
    }
    public void SetFloorCard(){
        
    }
    
}
