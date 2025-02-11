using UnityEngine;

[CreateAssetMenu]
public class BuildingConfig : ScriptableObject
{
    public int Id => Prefab.Id;
    public Sprite Icon => Prefab.Sprite;
    public Vector2Int Size => Prefab.Size;

    public int Price;
    public Building Prefab;
}
