using System;

public interface IPopulationNeedBuilding
{
    Guid UnicId { get; }
    int NeedPopulationCount { get; }
    int CurrentPopulationCount { get; }
    bool MustBeFullPopulation { get; }

    void AddPopulation(int population);
    void RemovePopulation(int population);
}