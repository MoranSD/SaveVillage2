using UnityEngine;

public static class GameMath
{
    public static Vector3 MouseToWorld()
    {
        return G.CameraController.Camera.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector2Int MouseToGridWorld()
    {
        var worldMouse = MouseToWorld();
        var gridMouse = Vector2Int.zero;
        gridMouse.x = Mathf.RoundToInt(worldMouse.x);
        gridMouse.y = Mathf.RoundToInt(worldMouse.y);
        return gridMouse;
    }

    public static Vector2Int WorldToGridWorld(Vector3 position)
    {
        var gridPosition = Vector2Int.zero;
        gridPosition.x = Mathf.RoundToInt(position.x);
        gridPosition.y = Mathf.RoundToInt(position.y);
        return gridPosition;
    }
}
