public class FoodBuilding : PopulationNeedBuilding
{
    public int FoodCount;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<DayCompleteEvent>(OnDayComplete, 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<DayCompleteEvent>(OnDayComplete);
    }

    public override string GetInfo()
    {
        var info = $"brings {FoodCount} meals\nat the start of each day\n\n";

        info += base.GetInfo();

        return info;
    }

    private void OnDayComplete(DayCompleteEvent dayComplete)
    {
        if (CurrentPopulationCount < NeedPopulationCount)
            return;

        G.HungrySystem.AddFood(FoodCount);
    }
}