using System;
using UnityEngine;

public enum GamePlayer{
    none,
    one,
    two
}

static class PlayerFunctions
{
    public static GamePlayer NextPlayer(GamePlayer current){
        switch(current){
            case GamePlayer.one:
                return GamePlayer.two;
            case GamePlayer.two:
                return GamePlayer.one;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player", (int)current));
        }
    }
}