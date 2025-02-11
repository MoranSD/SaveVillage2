using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystemView : MonoBehaviour
{
    public event Action<int, Vector2Int> OnBuild;

    [SerializeField] private Transform buildingsPivot;
    [SerializeField] private BuildingIconView iconViewPrefab;
    [SerializeField] private DragBuilding dragPrefab;

    private List<BuildingIconView> buildings = new();
    private Dictionary<int, Sprite> buildingSprites = new();
    private DragBuilding dragBuilding;

    private void OnDestroy()
    {
        foreach (var building in buildings)
            building.OnDrag -= OnBeginDragBuilding;
    }

    private void Update()
    {
        if (dragBuilding == null) return;

        if (Input.GetMouseButton(0))
        {
            dragBuilding.transform.position = (Vector3)(Vector2)GameMath.MouseToGridWorld();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Destroy(dragBuilding.gameObject);
            dragBuilding = null;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnBuild?.Invoke(dragBuilding.Id, GameMath.MouseToGridWorld());
            Destroy(dragBuilding.gameObject);
            dragBuilding = null;
        }
    }

    public void SetBuildPanelActive(bool active) => buildingsPivot.gameObject.SetActive(active);
    public void ChangeBuildPanelActive(bool active) => buildingsPivot.gameObject.SetActive(active);

    public void SetAllowBuilding(int id)
    {
        foreach (var building in buildings)
            building.gameObject.SetActive(id == building.Id);
    }
    public void ResetAllowBuilding()
    {
        foreach (var building in buildings)
            building.gameObject.SetActive(true);
    }

    public void AddBuilding(int id, Sprite sprite, int price)
    {
        var building = Instantiate(iconViewPrefab, buildingsPivot);

        building.Id = id;
        building.PriceText.text = $"$ {price}";
        building.Image.sprite = sprite;
        building.OnDrag += OnBeginDragBuilding;

        buildings.Add(building);
        buildingSprites.Add(id, sprite);
    }

    private void OnBeginDragBuilding(int id)
    {
        if (dragBuilding != null) return;

        dragBuilding = Instantiate(dragPrefab, (Vector3)(Vector2)GameMath.MouseToGridWorld(), Quaternion.identity);
        dragBuilding.Id = id;
        dragBuilding.SpriteRenderer.sprite = buildingSprites[id];
    }
}