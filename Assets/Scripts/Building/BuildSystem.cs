using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    public event Action<Building> OnBuilt;
    public event Action<Building> OnDestroyed;

    [SerializeField] private BuildSystemView view;

    public List<Building> Buildings = new();

    private bool isAbleToBuild;
    private int allowBuildingId = -1;

    private void Awake()
    {
        G.BuildSystem = this;
    }

    private void Start()
    {
        var allBuildings = G.Main.BuildingConfigs;
        foreach (var building in allBuildings)
            view.AddBuilding(building.Id, building.Icon, building.Price);

        view.OnBuild += CreateBuilding;
    }

    private void OnDestroy()
    {
        view.OnBuild -= CreateBuilding;

        foreach (var building in Buildings)
            building.OnDestroyed -= OnBuildingDestroyed;
    }

    public void SetAllowBuilding(int id)
    {
        allowBuildingId = id;
        view.SetAllowBuilding(allowBuildingId);
    }
    public void ResetAllowBuilding()
    {
        allowBuildingId = -1;
        view.ResetAllowBuilding();
    }

    public void ChangeBuildingActive(bool state)
    {
        isAbleToBuild = state;
        view.ChangeBuildPanelActive(state);
    }

    public void SetBuildingActive(bool state)
    {
        isAbleToBuild = state;
        view.SetBuildPanelActive(state);
    }

    public void CreateBuilding(int id, Vector2Int pos)
    {
        if (isAbleToBuild == false) return;
        if (allowBuildingId != -1 && allowBuildingId != id) return;

        var buildingConfig = G.Main.BuildingConfigs.First(building => building.Id == id);

        if (G.Main.GameState.WorldGrid.CheckFreeSpace(pos, buildingConfig.Size) == false)
            return;

        if (G.Wallet.TryGetMoney(buildingConfig.Price) == false)
            return;

        var building = Instantiate(buildingConfig.Prefab, (Vector3)(Vector2)pos, Quaternion.identity);
        building.UnicId = Guid.NewGuid();
        building.OnDestroyed += OnBuildingDestroyed;
        Buildings.Add(building);
        G.Main.GameState.WorldGrid.FillSpace(pos, buildingConfig.Size);

        OnBuilt?.Invoke(building);
    }

    private void OnBuildingDestroyed(Building building)
    {
        Buildings.Remove(building);

        G.Main.GameState.WorldGrid.FreeSpace(GameMath.WorldToGridWorld(building.transform.position), building.Size);

        OnDestroyed?.Invoke(building);
    }
}
