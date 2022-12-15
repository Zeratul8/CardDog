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
    public const string PLUS_HAND = "PlusHand";
    public const string MINUS_HAND = "MinusHand";
    public const string POP_CARD = "PopCard";
    public const string SET_FLOOR_CARD = "SetFloorCard";
    public const string PLAY_NEXT = "PlayNext";
    public const string ZERO_HAND = "ZeroHand";
    public const string READY_TO_PLAY = "ReadyToPlay";
    public const string CHECK_SHAKE = "CheckShake";
    public const string DROP_BOMB = "DropBomb";


}
public enum ScoreState
{
    Go,
    Shake,
    Poop
}
