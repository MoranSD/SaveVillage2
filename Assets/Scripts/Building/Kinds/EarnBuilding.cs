using System;
public class EarnBuilding : PopulationNeedBuilding
{
    public int RevenueCount;

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
        var info = $"earns {RevenueCount} money\nat the end of each day\n\n";

        info += base.GetInfo();

        return info;
    }

    private void OnDayComplete()
    {
        if (CurrentPopulationCount < NeedPopulationCount)
            return;

        G.Wallet.AddMoney(RevenueCount);
    }
}
