using System;
using UnityEngine;

public class PopulationNeedBuilding : Building, IPopulationNeedBuilding
{
    [field: SerializeField] public int NeedPopulationCount { get; private set; }
    public int CurrentPopulationCount { get; private set; }

    public override string GetInfo()
    {
        var info = base.GetInfo();

        info +=
                $"Need population: " +
                $"{CurrentPopulationCount}/{NeedPopulationCount}\n" +
                $"Established population:\n";

        var establishedPopulation = G.PopulationSystem.GetAllEstablishedPopulation(this);

        foreach (var established in establishedPopulation)
            info += $"{established.Count} from {GameUtils.GetShortStringId(established.From)}...\n";

        return info;
    }

    public void AddPopulation(int population)
    {
        CurrentPopulationCount += population;
    }

    public void RemovePopulation(int population)
    {
        CurrentPopulationCount -= population;
    }
}
