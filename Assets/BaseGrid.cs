using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TNRD;
using UnityEngine;

enum GridState{
    turnOne,
    oneToTwo,
    turnTwo,
    twoToOne
}
public class BaseGrid : MonoBehaviour
{
    GridState state = GridState.turnOne; 
    public GamePlayer currentPlayer = GamePlayer.one;
    public GamePlayer[,] tokens = new GamePlayer[7, 6];
    public SerializableInterface<IGridDisplay>[] gridDisplays = new SerializableInterface<IGridDisplay>[0];
    public SerializableInterface<IGridVictory>[] victoryListeners = new SerializableInterface<IGridVictory>[0];
    public SerializableInterface<IGridTurns>[] turnListeners = new SerializableInterface<IGridTurns>[0];


    public Vector2Int dimensions{
        get => new Vector2Int(tokens.GetLength(0),tokens.GetLength(1));
    }
    
    int LowestFreeSlot(int column){
        for(int i = 0; i < tokens.GetLength(1); i++){
            if (tokens[column, i] == GamePlayer.none){
                return i;
            }
        }
        return tokens.GetLength(1);
    }

    bool PlayersTurn(GamePlayer player)
    {
        return player == currentPlayer;
    }

    public void Drop(GamePlayer player, int column){
        if (player != currentPlayer){
            throw new NotYourTurnException();
        }
        int row = LowestFreeSlot(column);
        if(row==tokens.GetLength(1)){
            throw new IndexOutOfRangeException("Tried To Insert Token At The Top of a Full Column");
        }
        var allDroped = new CompletionCount(gridDisplays.Count(), this, column, row);
        foreach (var serializedDisplay in gridDisplays)
        {
            IGridDisplay display = serializedDisplay.Value;
            display.DropToken(currentPlayer, column, row, allDroped);
        }
        tokens[column, row] = currentPlayer;
    }
    public void PassTurn(){
        currentPlayer = PlayerFunctions.NextPlayer(currentPlayer);
        switch (currentPlayer){
            case GamePlayer.one:
                state = GridState.turnOne;
                break;
            case GamePlayer.two:
                state = GridState.turnTwo;
                break;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player", (int)currentPlayer));
        } 
    }
    IEnumerator AfterDrops(CompletionCount drops){
        yield return drops.IsCompleted();
    }
    void TokenDropped(int column, int row){
        if (CheckVictory(new Vector2Int(column, row))){
            foreach (var serializedListener in victoryListeners)
            {
                IGridVictory victoryListener = serializedListener.Value;
                victoryListener.Victory(currentPlayer);
            }
        }else{
            PassTurn();
        }
        
    }

    public class CompletionCount{
        int totalDisplays;
        int finishedDisplays=0;
        BaseGrid grid;
        int column;
        int row;
        public CompletionCount(int totalDisplays, BaseGrid grid, int column, int row){
            this.totalDisplays = totalDisplays;
            this.grid = grid;
            this.column = column;
            this.row = row;
        }
        public bool IsCompleted(){
            return finishedDisplays >= totalDisplays;
        }
        public void markCompleted(){
            finishedDisplays++;
            if (IsCompleted()){
                grid.TokenDropped(column, row);
            }
        }
    }
    
    // Update is called once per frame
    bool CheckVictory(Vector2Int pos){
        if (checkVictoryDirection(pos, Vector2Int.left)) return true;
        if (checkVictoryDirection(pos, new Vector2Int(1, 1))) return true;
        if (checkVictoryDirection(pos, new Vector2Int(1, -1))) return true;
        return checkVictoryDirection(pos, Vector2Int.up);
    }

    bool checkVictoryDirection(Vector2Int start, Vector2Int direction){
        int count = 0; 
        try {
            foreach (int i in Enumerable.Range(1,3)){
                Vector2Int current = start+(direction*i);
                if(slotMatchesPlayer(current.x, current.y, currentPlayer)){
                    count++;
                } else {
                    break;
                }
            }
        }
        catch (IndexOutOfRangeException){}
        try {
            foreach (int i in Enumerable.Range(1,3)){
                Vector2Int current = start-(direction*i);
                if(slotMatchesPlayer(current.x, current.y, currentPlayer)){
                    count++;
                } else {
                    break;
                }
            }
        }
        catch (IndexOutOfRangeException){}
        return count>=3;
    }
    bool slotMatchesPlayer(int x, int y, GamePlayer player){
        return tokens[x, y] == player;
    }
    
    void Start(){
        foreach (var serializedDisplays in gridDisplays){
            IGridDisplay display = serializedDisplays.Value;
            display.Init(tokens.GetLength(0), tokens.GetLength(1));
        }
    }
}

public class NotYourTurnException: Exception{
    public NotYourTurnException()
    {
    }

    public NotYourTurnException(string message)
        : base(message)
    {
    }

    public NotYourTurnException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
