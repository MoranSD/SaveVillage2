using System;
using UnityEngine;

public class PopulationBuilding : Building, IPopulationBuilding
{
    public event Action<IPopulationBuilding, int> OnLostPopulation;

    [field: SerializeField] public int Population { get; private set; }
    public int CurrentFoodCount { get; private set; }

    public int NeedFoodCount => Population - CurrentFoodCount;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<DayBeginEvent>(OnDayBegin);

        CurrentFoodCount = Population;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<DayBeginEvent>(OnDayBegin);
    }

    public void Feed(int count)
    {
        CurrentFoodCount += count;
    }

    public void RemoveOnePopulation()
    {
        Population--;
        OnLostPopulation?.Invoke(this, 1);
    }

    public override string GetInfo()
    {
        var info = 
            $"Contains {Population} population\n" +
            $"Food {CurrentFoodCount}\n\n";

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

    private void OnDayBegin(DayBeginEvent dayBegin)
    {
        //сначала забираем еду
        CurrentFoodCount -= Population;
        int oldPopulation = Population;

        //если еда в отрицательном значении,
        //то убиваем по сути тех,
        //кому она не досталась
        //(т.е. отрицательное значение CurrentFoodCount)
        if (CurrentFoodCount < 0)
        {
            Population -= Mathf.Abs(CurrentFoodCount);

            if(Population <= 0)
            {
                Population = 0;
                OnLostPopulation?.Invoke(this, oldPopulation);
                Kill();
            }
            else
            {
                OnLostPopulation?.Invoke(this, oldPopulation - Population);
            }
        }
    }
}
