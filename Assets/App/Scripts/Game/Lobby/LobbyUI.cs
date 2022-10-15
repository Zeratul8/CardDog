using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    public GameObject roomObject;
    public RectTransform contentRect;
    public float padding;
    public float primaryPadding;
    public int roomCount;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform objectRect = roomObject.GetComponent<RectTransform>();
        float width = objectRect.rect.width;
        contentRect.sizeDelta = new Vector2(primaryPadding * 2 + roomCount * (width + padding), contentRect.rect.height);
        for (int i = 0; i < roomCount; i++)
        {
            GameObject go = Instantiate(roomObject, contentRect);
            RectTransform roomRect = go.GetComponent<RectTransform>();
            roomRect.anchorMin = new Vector2(0, 0.5f);
            roomRect.anchorMax = new Vector2(0, 0.5f);
            roomRect.anchoredPosition = new Vector2(primaryPadding + 0.5f*width + i * (width + padding), 0);

            TextMeshProUGUI tm = go.GetComponentInChildren<TextMeshProUGUI>();
            tm.text = i.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
