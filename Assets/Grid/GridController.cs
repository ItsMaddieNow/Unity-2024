using UnityEngine;


[RequireComponent(typeof(MeshCollider))]
public class GridController : MonoBehaviour, IGridMessages
{
    public BaseGrid grid;
    void Start()
    {
        if (grid==null){
            grid = transform.parent.GetComponent<BaseGrid>();
        }
        if (grid==null){
            Debug.Log("'grid' is unassigned and couldn't find component of 'BaseGrid' in parent");
        }
    }
    public void ClickGrid(Vector2 pos)
    {
        Vector2 scaled = (pos * grid.dimensions);
        Vector2Int indexes = Vector2Int.Min(new Vector2Int(Mathf.FloorToInt(scaled.x), Mathf.FloorToInt(scaled.y)), grid.dimensions-Vector2Int.one);
        grid.Drop(grid.currentPlayer, indexes.x);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
