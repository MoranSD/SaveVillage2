using UnityEngine;

public class PopulationBuilding : Building, IPopulationBuilding
{
    [field: SerializeField] public int Population { get; private set; }

    public override string GetInfo()
    {
        var info = $"Contains {Population} population\n\n";

        info += base.GetInfo();

        int remainingPopulation = G.PopulationSystem.GetRemainingPopulation(this);

        info +=
            $"Remaining population: {remainingPopulation}\n" +
            $"Employed Population:\n";

        var establishedPopulation = G.PopulationSystem.GetAllEmployeedPopulation(this);

        foreach (var established in establishedPopulation)
            info += $"{established.Count} to {GameUtils.GetShortStringId(established.To)}...\n";

        return info;
    }
}
