public class FoodBuilding : PopulationNeedBuilding
{
    public int FoodCount;

    protected override void Awake()
    {
        base.Awake();
        G.Main.OnDayComplete += OnDayComplete;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        G.Main.OnDayComplete -= OnDayComplete;
    }

    public override string GetInfo()
    {
        var info = $"brings {FoodCount} meals\nat the end of each day\n\n";

        info += base.GetInfo();

        return info;
    }

    private void OnDayComplete()
    {
        if (CurrentPopulationCount < NeedPopulationCount)
            return;

        //add food
    }
}