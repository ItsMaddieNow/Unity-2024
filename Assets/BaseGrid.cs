using UnityEngine;

public class BaseGrid : MonoBehaviour
{
    public GamePlayer currentPlayer;
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

    bool PlayersTurn(GamePlayer player)
    {
        return player == currentPlayer;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
