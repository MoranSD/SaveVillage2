using System.Linq;
using UnityEngine;

public class HungrySystem : MonoBehaviour
{
    private void Awake()
    {
        G.HungrySystem = this;
    }

    private void Start()
    {
        EventBus.Subscribe<DayCompleteEvent>(OnDayComplete);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<DayCompleteEvent>(OnDayComplete);
    }

    public void AddFood(int count)
    {
        G.Main.GameState.FoodCount += count;
        G.StatsDrawer.UpdateDraw();
    }

    private void OnDayComplete(DayCompleteEvent dayComplete)
    {
        while (true)
        {
            if (G.Main.GameState.FoodCount == 0) break;

            var hungryBuildings = G.BuildSystem.Buildings
            .Where(x => x is IPopulationBuilding)
            .Select(x => (IPopulationBuilding)x)
            .Where(x => x.NeedFoodCount > 0);

            int count = hungryBuildings.Count();

            if (count == 0) break;

            var randomBuilding = hungryBuildings.ElementAt(UnityEngine.Random.Range(0, count));
            var foodCount = Mathf.Min(randomBuilding.NeedFoodCount, G.Main.GameState.FoodCount);
            randomBuilding.Feed(foodCount);

            G.Main.GameState.FoodCount -= foodCount;
        }

        G.StatsDrawer.UpdateDraw();
    }
}
