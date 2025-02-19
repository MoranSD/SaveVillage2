﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopulationSystem : MonoBehaviour
{
    public event Action OnPopulationEmployeesUpdated;

    public Dictionary<Guid, int> BuildingsRemainingPopulation = new();//оставшееся количество популяции в каждом здании
    public List<EmployeedPopulation> EmployeedPopulation = new();//выданные рабочие из здания в здание и количество
    public List<IPopulationNeedBuilding> PopulationNeedBuildings = new();//здания, которым нужна популяция для работы

    private List<IPopulationBuilding> populationBuildings = new();

    private void Awake()
    {
        G.PopulationSystem = this;
    }

    private void Start()
    {
        G.BuildSystem.OnBuilt += OnBuiltBuilding;
        G.BuildSystem.OnDestroyed += OnBuildingDestroyed;
    }

    private void OnDestroy()
    {
        G.BuildSystem.OnBuilt -= OnBuiltBuilding;
        G.BuildSystem.OnDestroyed -= OnBuildingDestroyed;

        foreach (var populationBuilding in populationBuildings)
            populationBuilding.OnLostPopulation -= OnBuildingLostPopulation;
    }

    public int GetEmployeedPopulationCount() => EmployeedPopulation.Sum(x => x.Count);
    public int GetRemainingPopulation(IPopulationBuilding building) => BuildingsRemainingPopulation[building.UnicId];

    public List<EmployeedPopulation> GetAllEmployeedPopulation(IPopulationBuilding building)
    {
        var employees = new List<EmployeedPopulation>();

        foreach (var established in EmployeedPopulation)
        {
            if (established.From != building.UnicId) continue;

            employees.Add(established);
        }

        return employees;
    }
    public List<EmployeedPopulation> GetAllEstablishedPopulation(IPopulationNeedBuilding populationNeed)
    {
        var employees = new List<EmployeedPopulation>();

        foreach (var established in EmployeedPopulation)
        {
            if (established.To != populationNeed.UnicId) continue;

            employees.Add(established);
        }

        return employees;
    }

    private void OnBuildingLostPopulation(IPopulationBuilding building, int lostCount)
    {
        G.Main.GameState.Population -= lostCount;

        if (BuildingsRemainingPopulation[building.UnicId] >= lostCount)
        {
            BuildingsRemainingPopulation[building.UnicId] -= lostCount;
        }
        else
        {
            int remainingToRemoveCount = lostCount - BuildingsRemainingPopulation[building.UnicId];
            BuildingsRemainingPopulation[building.UnicId] = 0;

            for (int i = EmployeedPopulation.Count - 1; i >= 0; i--)
            {
                var established = EmployeedPopulation[i];

                if (established.From != building.UnicId) continue;

                var to = PopulationNeedBuildings.First(x => x.UnicId == established.To);

                if(established.Count >= remainingToRemoveCount)
                {
                    to.RemovePopulation(remainingToRemoveCount);

                    if(remainingToRemoveCount == established.Count)
                        EmployeedPopulation.RemoveAt(i);

                    break;
                }
                else
                {
                    remainingToRemoveCount -= established.Count;
                    to.RemovePopulation(established.Count);
                    EmployeedPopulation.RemoveAt(i);

                    if (remainingToRemoveCount == 0)
                        break;
                }
            }

            UpdatePopulationEmployees();
        }
    }

    private void OnBuiltBuilding(Building building)
    {
        if (building is IPopulationBuilding populationBuilding)
        {
            G.Main.GameState.Population += populationBuilding.Population;
            BuildingsRemainingPopulation.Add(building.UnicId, populationBuilding.Population);

            populationBuildings.Add(populationBuilding);
            populationBuilding.OnLostPopulation += OnBuildingLostPopulation;
        }
        else if (building is IPopulationNeedBuilding populationNeedBuilding)
        {
            PopulationNeedBuildings.Add(populationNeedBuilding);
        }

        UpdatePopulationEmployees();
        G.StatsDrawer.UpdateDraw();
    }
    private void OnBuildingDestroyed(Building building)
    {
        if (building is IPopulationBuilding populationBuilding)
        {
            G.Main.GameState.Population -= populationBuilding.Population;
            BuildingsRemainingPopulation.Remove(building.UnicId);
            RemoveEstablishedPopulation(populationBuilding);

            populationBuildings.Remove(populationBuilding);
            populationBuilding.OnLostPopulation -= OnBuildingLostPopulation;
        }
        else if (building is IPopulationNeedBuilding populationNeedBuilding)
        {
            PopulationNeedBuildings.Remove(populationNeedBuilding);
            GetBackEstablishedPopulation(populationNeedBuilding);
        }

        UpdatePopulationEmployees();
        G.StatsDrawer.UpdateDraw();
    }

    private void GetBackEstablishedPopulation(IPopulationNeedBuilding building)
    {
        for (int i = EmployeedPopulation.Count - 1; i >= 0; i--)
        {
            var established = EmployeedPopulation[i];

            if (established.To != building.UnicId) continue;

            BuildingsRemainingPopulation[established.From] += established.Count;
            EmployeedPopulation.RemoveAt(i);
        }
    }
    private void RemoveEstablishedPopulation(IPopulationBuilding building)
    {
        for (int i = EmployeedPopulation.Count - 1; i >= 0; i--)
        {
            var established = EmployeedPopulation[i];

            if (established.From != building.UnicId) continue;

            var to = PopulationNeedBuildings.First(x => x.UnicId == established.To);

            to.RemovePopulation(established.Count);
            EmployeedPopulation.RemoveAt(i);
        }
    }

    private void UpdatePopulationEmployees()
    {
        bool updated = false;

        foreach (var populationNeed in PopulationNeedBuildings)
        {
            if (populationNeed.CurrentPopulationCount >= populationNeed.NeedPopulationCount) continue;

            int needCount = populationNeed.NeedPopulationCount - populationNeed.CurrentPopulationCount;
            bool populated = false;

            foreach (var remainingPopulation in BuildingsRemainingPopulation)
            {
                if (remainingPopulation.Value >= needCount)
                {
                    MovePopulationFromBuilding(populationNeed, remainingPopulation.Key, needCount);
                    populated = true;
                    updated = true;
                    break;
                }
            }

            if (populated) continue;

            int totalRemainingPopulationCount = BuildingsRemainingPopulation.Sum(x => x.Value);
            if (totalRemainingPopulationCount >= needCount)
            {
                MovePopulationFromAllBuildings(populationNeed);
                updated = true;
            }
            else if (populationNeed.MustBeFullPopulation == false)
            {
                var remainingPopulationBuildingsIds = BuildingsRemainingPopulation.Select(x => x.Key).ToList();
                foreach (var id in remainingPopulationBuildingsIds)
                    if (BuildingsRemainingPopulation[id] > 0)
                        MovePopulationFromBuilding(populationNeed, id, BuildingsRemainingPopulation[id]);
            }
        }

        if(updated)
            OnPopulationEmployeesUpdated?.Invoke();
    }
    private void MovePopulationFromBuilding(IPopulationNeedBuilding populationNeed, Guid populationBuildingId, int count)
    {
        EmployeedPopulation.Add(new()
        {
            From = populationBuildingId,
            To = populationNeed.UnicId,
            Count = count,
        });

        BuildingsRemainingPopulation[populationBuildingId] -= count;
        populationNeed.AddPopulation(count);
    }
    private void MovePopulationFromAllBuildings(IPopulationNeedBuilding populationNeed)
    {
        while (true)
        {
            foreach (var remainingPopulation in BuildingsRemainingPopulation)
            {
                int needCount = populationNeed.NeedPopulationCount - populationNeed.CurrentPopulationCount;

                if (needCount == 0) return;
                if (remainingPopulation.Value == 0) continue;

                MovePopulationFromBuilding(populationNeed, remainingPopulation.Key, Mathf.Min(needCount, remainingPopulation.Value));
                break;
            }
        }
    }
}

public struct EmployeedPopulation
{
    public Guid From;
    public Guid To;
    public int Count;
}
