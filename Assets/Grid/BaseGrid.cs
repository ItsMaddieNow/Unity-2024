using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
    IGridState state = new TurnState(); 
    private int completedColumns = 0;
    public GamePlayer currentPlayer = GamePlayer.one;
    [HideInInspector]
    public GamePlayer[,] tokens = new GamePlayer[7, 6];
    [ShowInInspector]
    public List<IGridDisplay> gridDisplays = new List<IGridDisplay>();
    [ShowInInspector]
    public List<IGridEnd> endListeners = new List<IGridEnd>();
    [ShowInInspector]
    public List<IGridTurns> turnListeners = new List<IGridTurns>();


    public Vector2Int dimensions{
        get => new Vector2Int(tokens.GetLength(0),tokens.GetLength(1));
    }
    

    void Update(){
        state.Update(this);
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
        return state.IsPlayersTurn(this, player);
    }

    public void Drop(GamePlayer player, int column){
        state.Drop(this, player, column);
    }
    public void PassTurn(){
        currentPlayer = PlayerFunctions.NextPlayer(currentPlayer);
    }

    public class StateCompletion{
        int remainingDisplays;
        public StateCompletion(int totalDisplays){
            this.remainingDisplays = totalDisplays;
        }
        public bool IsDone(){
            return remainingDisplays <= 0;
        }
        public void markCompleted(){
            remainingDisplays--;
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
        foreach (IGridDisplay display in gridDisplays){
            display.Init(tokens.GetLength(0), tokens.GetLength(1));
        }
    }
    void ChangeState(IGridState newState){
        state.Exit(this);
        state = newState;
        state.Enter(this);
    }
    public void Reset(){
        tokens = new GamePlayer[7, 6];
        completedColumns = 0;
        currentPlayer = GamePlayer.one;
        ChangeState(new TurnState());
        foreach (IGridDisplay display in gridDisplays)
        {
            display.Clear();
        }
        foreach (IGridTurns turnListener in turnListeners)
        {
            turnListener.InstantTransition(currentPlayer);
        }
    }
    public interface IGridState{
        public void Update(BaseGrid grid);
        public void Exit(BaseGrid grid);
        public void Enter(BaseGrid grid){}
        public void Drop(BaseGrid grid, GamePlayer playerDropping, int column){
            throw new NotYourTurnException();
        }
        public bool IsPlayersTurn(BaseGrid grid, GamePlayer player){
            return false;
        }
    }
    public class TurnState:IGridState{
        public void Update(BaseGrid grid){
            
        }
        public void Exit(BaseGrid grid){

        }
        public void Ee(BaseGrid grid){

        }
        public void Drop(BaseGrid grid, GamePlayer playerDropping, int column){
            if (!grid.PlayersTurn(playerDropping)){
                throw new NotYourTurnException();
            } else {
                grid.ChangeState(new DroppingState(grid, playerDropping, column));
            }
        }
        public bool IsPlayersTurn(BaseGrid grid, GamePlayer player){
            return grid.currentPlayer == player;
        }
    }

    public class DroppingState : IGridState
    {
        int column, row;
        StateCompletion stateCompletion;
        public DroppingState(BaseGrid grid, GamePlayer playerDropping, int column){
            this.column = column;
            row = grid.LowestFreeSlot(column);
            if(row==grid.tokens.GetLength(1)){
                throw new IndexOutOfRangeException("Tried To Insert Token At The Top of a Full Column");
            }
            if(row-1==grid.tokens.GetLength(1)){
                grid.completedColumns++;
            }
            stateCompletion = new StateCompletion(grid.gridDisplays.Count());
            foreach (IGridDisplay display in grid.gridDisplays)
            {
                display.DropToken(playerDropping, column, row, stateCompletion);
            }
            grid.tokens[column, row] = playerDropping;
        }
        public void Exit(BaseGrid grid)
        {
            
        }

        public void Update(BaseGrid grid)
        {   
            if (stateCompletion.IsDone()){
                if (grid.CheckVictory(new Vector2Int(column, row))){
                    grid.ChangeState(new VictoryState());
                }
                else if(grid.completedColumns>=grid.tokens.GetLength(0)){
                    grid.ChangeState(new EndState());
                }
                else {
                    //grid.PassTurn();
                    grid.ChangeState(new SwapState());
                }
            }
        }
    }
    public class SwapState : IGridState
    {
        StateCompletion stateCompletion;
        public void Exit(BaseGrid grid)
        {

        }
        public void Enter(BaseGrid grid){
            grid.PassTurn();
            stateCompletion = new StateCompletion(grid.turnListeners.Count());
            foreach (IGridTurns turnListener in grid.turnListeners)
            {
                turnListener.PlayerTransition(grid.currentPlayer, stateCompletion);
            }
            
        }
        public void Update(BaseGrid grid)
        {
            if (stateCompletion.IsDone()){
                grid.ChangeState(new TurnState());
            }
        }
    }
    public class VictoryState : IGridState
    {
        public void Exit(BaseGrid grid)
        {
            
        }

        public void Update(BaseGrid grid)
        {
            
        }
        public void Enter(BaseGrid grid){
            foreach (IGridEnd endListener in grid.endListeners)
            {
                endListener.Victory(grid.currentPlayer);
            }
        }
    }
    public class EndState: IGridState
    {
        public void Exit(BaseGrid grid){
            
        }

        public void Update(BaseGrid grid){
            
        }
        public void Enter(BaseGrid grid){
            foreach (IGridEnd endListener in grid.endListeners)
            {
                endListener.End();
            }
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

