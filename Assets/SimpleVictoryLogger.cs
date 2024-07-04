using System;
using UnityEngine;

class SimpleVictoryLogger : MonoBehaviour, IGridVictory
{
    public void Victory(GamePlayer player)
    {
        Debug.Log(String.Format("Player {} Wins!", player.ToString()));
    }
}