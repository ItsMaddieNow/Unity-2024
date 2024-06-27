using System;
using UnityEngine;

public enum GamePlayer{
    red=0,
    yellow=1
}

static class PlayerFunctions
{
    public static GamePlayer NextPlayer(GamePlayer current){
        return (GamePlayer)(((int)current+1)%2);
    }
}