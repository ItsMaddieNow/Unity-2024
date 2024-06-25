using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public interface IGridMessages: IEventSystemHandler
{
    void DropToken(Vector2 pos);
}
