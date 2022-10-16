using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CARD_TYPE{
    NORMAL, //피
    BAND, //띠
    LIGHT, //광
    SPECIAL, //열끗? 멍?
    JOKER // 특수카드
}
public enum SPECIAL_CARD{
    NORMAL, //보통
    FIVEVBIRD, //고도리
    BLUEBAND, //청단
    REDBAND, //홍단
    GRASSBAND, //초단

}

[Serializable]
public class CardClass
{
    public int index;
    public int month;
    public int score;
    public SPECIAL_CARD sCard;
    public CARD_TYPE type;
}

