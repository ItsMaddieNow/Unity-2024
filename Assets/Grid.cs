using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Grid : MonoBehaviour, IGridMessages
{
    public Vector2Int dimensions = new Vector2Int(7, 6);
    public void DropToken(Vector2 pos){
        Vector2 scaled = (pos * dimensions);
        Vector2Int indexes = Vector2Int.Min(new Vector2Int(Mathf.FloorToInt(scaled.x), Mathf.FloorToInt(scaled.y)), dimensions-Vector2Int.one);
        print("Dropped Token: "+indexes);
    }

}
