using System;
public class EarnBuilding : PopulationNeedBuilding
{
    public int RevenueCount;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<DayBeginEvent>(OnDayBegin);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<DayBeginEvent>(OnDayBegin);
    }

    public override string GetInfo()
    {
        var info = $"earns {RevenueCount} money\nat the start of each day\n\n";

        info += base.GetInfo();

        return info;
    }

    private void OnDayBegin(DayBeginEvent dayBegin)
    {
        if (CurrentPopulationCount < NeedPopulationCount)
            return;

        G.Wallet.AddMoney(RevenueCount);
    }
}
