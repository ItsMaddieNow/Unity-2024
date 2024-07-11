using UnityEngine;


public interface IGridDisplay
{
    public void DropToken(GamePlayer player, int column, int row, BaseGrid.StateCompletion completion);
    public void Init(int width, int height);
    public void Clear();
}

public interface IGridTurns{
    public void PlayerTransition(GamePlayer player, BaseGrid.StateCompletion completion);
    public void InstantTransition(GamePlayer player);
}

public interface IGridEnd{
    public void Victory(GamePlayer player){}
    public void End(){}
    public void Reset(){}
}