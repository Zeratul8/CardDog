using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public enum SceneName
    {
        Title = 0,
        Lobby,
        Game
    }
    public const string START_GAME = "StartGame";
    public const string FINISH_GAME = "FinishGame";
    public const string ADD_HAND = "AddHand";
    public const string PUT_CARD = "PutCard";
}
