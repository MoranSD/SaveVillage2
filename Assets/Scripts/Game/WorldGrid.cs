using System;
using UnityEngine;

public class WorldGrid
{
    public Vector2Int WorldSize { get; }
    public bool[,] grid;

    public WorldGrid(Vector2Int worldSize)
    {
        WorldSize = worldSize;
        grid = new bool[worldSize.x, worldSize.y];
    }

    public bool CheckFreeSpace(Vector2Int centerPosition, Vector2Int buildingSize)
    {
        BuildingBounds bounds = CalculateBuildingBounds(centerPosition, buildingSize);

        if (!(bounds.StartX >= 0 &&
               bounds.EndX <= grid.GetLength(0) &&
               bounds.StartY >= 0 &&
               bounds.EndY <= grid.GetLength(1)))
            return false;

        for (int x = bounds.StartX; x < bounds.EndX; x++)
            for (int y = bounds.StartY; y < bounds.EndY; y++)
                if (grid[x, y])
                    return false;

        return true;
    }

    public void FillSpace(Vector2Int centerPosition, Vector2Int buildingSize)
    {
        if (!CheckFreeSpace(centerPosition, buildingSize))
            return;

        BuildingBounds bounds = CalculateBuildingBounds(centerPosition, buildingSize);

        for (int x = bounds.StartX; x < bounds.EndX; x++)
        {
            for (int y = bounds.StartY; y < bounds.EndY; y++)
            {
                grid[x, y] = true;
            }
        }
    }

    public void FreeSpace(Vector2Int centerPosition, Vector2Int buildingSize)
    {
        BuildingBounds bounds = CalculateBuildingBounds(centerPosition, buildingSize);

        for (int x = bounds.StartX; x < bounds.EndX; x++)
        {
            for (int y = bounds.StartY; y < bounds.EndY; y++)
            {
                grid[x, y] = false;
            }
        }
    }

    private BuildingBounds CalculateBuildingBounds(Vector2Int center, Vector2Int size)
    {
        int halfWidth = Mathf.FloorToInt(size.x / 2f);
        int halfHeight = Mathf.FloorToInt(size.y / 2f);

        return new BuildingBounds(
            center.x - halfWidth,
            center.x + halfWidth + (size.x % 2 == 0 ? 0 : 1),
            center.y - halfHeight,
            center.y + halfHeight + (size.y % 2 == 0 ? 0 : 1)
        );
    }

    private struct BuildingBounds
    {
        public readonly int StartX;
        public readonly int EndX;
        public readonly int StartY;
        public readonly int EndY;

        public BuildingBounds(int startX, int endX, int startY, int endY)
        {
            StartX = startX;
            EndX = endX;
            StartY = startY;
            EndY = endY;
        }
    }
}
