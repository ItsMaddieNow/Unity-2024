using System;
using UnityEngine;

public class GridDisplay : MonoBehaviour, IGridDisplay
{
    private Vector2Int dimensions = new Vector2Int();
    public GameObject tokenPrefab;
    
    
    public void Init(int width, int height){
        dimensions = new Vector2Int(width, height);
    }
    public void DropToken(GamePlayer player, int column, int row, BaseGrid.StateCompletion dropCompletion)
    {
        Vector2 gridPosition = new Vector2(column, row)-((Vector2)dimensions/2)+Vector2.one*0.5f;
        float dropLevel = ((float)dimensions.y)/2 + 2.5f;
        print("dropped: " + dropLevel + " target: " + gridPosition.y);
        GameToken token = Instantiate(tokenPrefab, transform.localToWorldMatrix* new Vector4(gridPosition.x, dropLevel, 0, 1), Quaternion.identity, transform).GetComponent<GameToken>();
        token.colour = player;
        token.Drop(gridPosition.y, dropCompletion);
    }

    

    public void Clear()
    {
        GameToken[] children = transform.GetComponentsInChildren<GameToken>();
        for(int i = 0; i<children.Length; i++){
            Destroy(children[i].gameObject);
        }
    }

    public void Victory()
    {
        throw new NotImplementedException();
    }
}
