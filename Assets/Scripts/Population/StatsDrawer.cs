using TMPro;
using UnityEngine;

public class StatsDrawer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statsText;
    [Header("Building stats")]
    [SerializeField] private Transform buildingStatsPanel;
    [SerializeField] private TextMeshProUGUI buildingStatsText;

    private DrawStatsType activeDraw = DrawStatsType.none;

    private void Awake() => G.StatsDrawer = this;
    private void Start() => UpdateDraw();
    private void Update()
    {
        if (activeDraw == DrawStatsType.none) return;

        switch (activeDraw)
        {
            case DrawStatsType.building:
                buildingStatsPanel.transform.position = Input.mousePosition;
                break;
        }
    }

    public void UpdateDraw()
    {
        statsText.text = 
            $"Day {G.Main.GameState.Day}\n" +
            $"$ {G.Main.GameState.MoneyCount}\n" +
            $"Population {G.Main.GameState.Population}\n" +
            $"Empoleed population {G.PopulationSystem.GetEmployeedPopulationCount()}\n" +
            $"\n" +
            $"Skip smart wait - T\n" +
            $"Skip day - D\n" +
            $"Destroy building - B\n" +
            $"Add money (10) - M";
    }

    public void DrawStats(Building building)
    {
        buildingStatsPanel.transform.position = Input.mousePosition;

        buildingStatsText.text = building.GetInfo();

        buildingStatsPanel.gameObject.SetActive(true);

        if (activeDraw != DrawStatsType.none && activeDraw != DrawStatsType.building)
            HideStats(activeDraw);

        activeDraw = DrawStatsType.building;
    }

    public void HideStats(DrawStatsType type)
    {
        switch (type)
        {
            case DrawStatsType.building:
                buildingStatsPanel.gameObject.SetActive(false);
                break;
        }
        activeDraw = DrawStatsType.none;
    }

    public enum DrawStatsType
    {
        none,
        building,
    }

    //тут можно будет просить отрисовку инфы для различных сущностей
    //к примеру: DrawStats(Building building) HideStats(StatsType type)
    //тут он поверх отрисует плашку и при надобности закроет ее по указанному типу
}