using UnityEngine;

[CreateAssetMenu]
public class GameConfig : ScriptableObject
{
    public Vector2Int WorldSize;
    public int StartMoney;
    public BuildingConfig[] BuildingConfigs;
}
