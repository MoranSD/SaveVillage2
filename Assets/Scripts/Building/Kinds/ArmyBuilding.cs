using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArmyBuilding : PopulationNeedBuilding
{
    [HideInInspector] public List<WarriorEntity> AliveWarriors = new();
    public override bool MustBeFullPopulation => false;
    public bool IsAnyDead => AliveWarriors.Count < CurrentPopulationCount;

    [SerializeField] private float respawnRate;
    [SerializeField] private WarriorEntity warriorEntityPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private List<int> freeSpawnPoints;
    private float respawnTime;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<DayCompleteEvent>(OnDayComplete);
        EventBus.Subscribe<DayBeginEvent>(OnDayBegin);

        freeSpawnPoints = new List<int>(spawnPoints.Length);
        for (int i = 0; i < spawnPoints.Length; i++)
            freeSpawnPoints.Add(i);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<DayCompleteEvent>(OnDayComplete);
        EventBus.Unsubscribe<DayBeginEvent>(OnDayBegin);

        foreach (var warrior in AliveWarriors)
            warrior.OnDie -= OnWarriorDie;
    }

    private void Update()
    {
        if (G.Main.IsNight == false) return;
        if (IsAnyDead == false) return;

        respawnTime -= Time.deltaTime;
        if(respawnTime <= 0)
        {
            respawnTime = respawnRate;
            SpawnWarrior();
        }
    }

    private void OnDayComplete(DayCompleteEvent dayComplete)
    {
        respawnTime = respawnRate;

        for (int i = 0; i < CurrentPopulationCount; i++)
            SpawnWarrior();
    }

    private void SpawnWarrior()
    {
        var randomFreeSpawnPointId = freeSpawnPoints[UnityEngine.Random.Range(0, freeSpawnPoints.Count)];
        freeSpawnPoints.Remove(randomFreeSpawnPointId);
        var warrior = Instantiate(warriorEntityPrefab, spawnPoints[randomFreeSpawnPointId].position, Quaternion.identity);
        warrior.SpawnPointId = randomFreeSpawnPointId;
        warrior.OnDie += OnWarriorDie;
        AliveWarriors.Add(warrior);
    }

    private void OnDayBegin(DayBeginEvent dayBegin)
    {
        for (int i = 0; i < AliveWarriors.Count;)
        {
            WarriorEntity warrior = AliveWarriors[0];
            warrior.ApplyDamage(warrior.Health);
        }
    }

    private void OnWarriorDie(Guid id)
    {
        var deadWarrior = AliveWarriors.First(x => x.UnicId == id);
        deadWarrior.OnDie -= OnWarriorDie;
        AliveWarriors.Remove(deadWarrior);
        freeSpawnPoints.Add(deadWarrior.SpawnPointId);
    }
}
