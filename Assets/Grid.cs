using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class Grid : MonoBehaviour, IGridMessages
{

    public void DropToken(Vector2 pos){
        print("Message recieved " + pos);
    }
}
