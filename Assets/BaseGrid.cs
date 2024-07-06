using System;
using System.Collections;
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
    IGridState state = new TurnState(); 
    public GamePlayer currentPlayer = GamePlayer.one;
    public GamePlayer[,] tokens = new GamePlayer[7, 6];
    public SerializableInterface<IGridDisplay>[] gridDisplays = new SerializableInterface<IGridDisplay>[0];
    public SerializableInterface<IGridVictory>[] victoryListeners = new SerializableInterface<IGridVictory>[0];
    public SerializableInterface<IGridTurns>[] turnListeners = new SerializableInterface<IGridTurns>[0];


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
        foreach (var serializedDisplays in gridDisplays){
            IGridDisplay display = serializedDisplays.Value;
            display.Init(tokens.GetLength(0), tokens.GetLength(1));
        }
    }
    void ChangeState(IGridState newState){
        state.Exit(this);
        state = newState;
        state.Enter(this);
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
            stateCompletion = new StateCompletion(grid.gridDisplays.Count());
            foreach (var serializedDisplay in grid.gridDisplays)
            {
                IGridDisplay display = serializedDisplay.Value;
                display.DropToken(playerDropping, column, row, stateCompletion);
            }
            grid.tokens[column, row] = playerDropping;
        }
        public void Exit(BaseGrid grid)
        {
            if (grid.CheckVictory(new Vector2Int(column, row))){
                foreach (var serializedListener in grid.victoryListeners)
                {
                    IGridVictory victoryListener = serializedListener.Value;
                    victoryListener.Victory(grid.currentPlayer);
                }
            }else{
                grid.PassTurn();
            }
        }

        public void Update(BaseGrid grid)
        {
            if (stateCompletion.IsDone()){
                grid.ChangeState(new SwapState());
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
            stateCompletion = new StateCompletion(grid.turnListeners.Count());
            foreach (var serializedListener in grid.turnListeners)
            {
                IGridTurns turnListener = serializedListener.Value;
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

