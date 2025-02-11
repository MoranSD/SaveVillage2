using System;

public interface IPopulationBuilding
{
    event Action<IPopulationBuilding, int> OnLostPopulation;

    int Population { get; }
    int NeedFoodCount { get; }
    Guid UnicId { get; }
    void Feed(int count);
}
