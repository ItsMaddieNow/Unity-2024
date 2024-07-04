using UnityEngine;


public interface IGridDisplay
{
    public void DropToken(GamePlayer player, int column, int row, BaseGrid.CompletionCount completion);
    public void Init(int width, int height);
    public void Clear();
}

public interface IGridTurns{
    public void PlayerTransition(GamePlayer player);
}

public interface IGridVictory{
    public void Victory(GamePlayer player);
}