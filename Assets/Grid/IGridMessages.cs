using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public interface IGridMessages: IEventSystemHandler
{
    void ClickGrid(Vector2 pos);
}
