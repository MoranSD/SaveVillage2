using UnityEngine;

public class FoodBuilding : PopulationNeedBuilding
{
    [field: SerializeField] public int NeedMoneyCount { get; private set; }

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
        var info = $"brings {FoodCount} meals\nat the start of each day\nfor {NeedMoneyCount} money\n\n";

        info += base.GetInfo();

        return info;
    }

    private void OnDayComplete(DayCompleteEvent dayComplete)
    {
        if (CurrentPopulationCount < NeedPopulationCount)
            return;
        if (G.Wallet.TryGetMoney(NeedMoneyCount) == false)
            return;

        G.HungrySystem.AddFood(FoodCount);
    }
}