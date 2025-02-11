using System;

public interface IPopulationNeedBuilding
{
    Guid UnicId { get; }
    int NeedPopulationCount { get; }
    int CurrentPopulationCount { get; }

    void AddPopulation(int population);
    void RemovePopulation(int population);
}