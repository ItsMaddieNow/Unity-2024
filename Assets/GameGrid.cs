using System;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class GameGrid : MonoBehaviour, IGridMessages
{
    public GamePlayer currentPlayer;
    public GameObject redToken;
    public GameObject yellowToken;
    public GameToken[,] tokens = new GameToken[7, 6];
    public Vector2Int dimensions{
        get => new Vector2Int(tokens.GetLength(0),tokens.GetLength(1));
    }

    int LowestFreeSlot(int column){
        for(int i = 0; i < tokens.GetLength(1); i++){
            if (tokens[column, i] == null){
                return i;
            }
        }
        return tokens.GetLength(1);
    }
    void PlaceToken(int x, int y){
        Vector2 gridPosition = new Vector2(x, y)-((Vector2)dimensions/2)+Vector2.one*0.5f;
        GameToken token = Instantiate(GetPlayerToken(currentPlayer), transform.localToWorldMatrix* new Vector4(gridPosition.x, gridPosition.y, 0, 1), quaternion.identity, transform).GetComponent<GameToken>();
        if (checkVictory(new Vector2Int(x, y))) print("Victory");
        currentPlayer = PlayerFunctions.NextPlayer(currentPlayer);
        token.drop();
        tokens[x, y] = token;
    }
    void DropToken(int column){
        for(int i = 0; i < tokens.GetLength(1); i++){
            if (tokens[column, i] == null){
                PlaceToken(column, i);
                break;
            }
        }
    }
    GameObject GetPlayerToken(GamePlayer player){
        switch(player){
            case GamePlayer.red:
                return redToken;
            case GamePlayer.yellow:
                return yellowToken;
            default:
                throw new ArgumentException(String.Format("{0} is not a valid Player Token", (int)player));
        }
    }
    bool checkVictory(Vector2Int pos){
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
    
    public void ClickGrid(Vector2 pos){
        Vector2 scaled = (pos * dimensions);
        Vector2Int indexes = Vector2Int.Min(new Vector2Int(Mathf.FloorToInt(scaled.x), Mathf.FloorToInt(scaled.y)), dimensions-Vector2Int.one);
        DropToken(indexes.x);
        print("Dropped Token: "+indexes);
    }

    bool slotMatchesPlayer(int x, int y, GamePlayer player){
        return tokens[x, y]!=null && tokens[x, y].getColour()==player;
    }
    
    void Victory(){

    }
}

    
