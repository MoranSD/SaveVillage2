using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    public bool IsWaveDead => Wave.Count == 0;
    public List<EnemyEntity> Wave = new();

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private EnemyEntity enemyEntityPrefab;

    private void Awake()
    {
        G.EnemySystem = this;
        EventBus.Subscribe<DayCompleteEvent>(OnDayComplete, 1);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<DayCompleteEvent>(OnDayComplete);
    }

    private void OnDayComplete(DayCompleteEvent @event) => SpawnWave();

    public void SpawnWave()
    {
        for (int i = 0; i < Mathf.Min(G.Main.GameState.Day + 1, spawnPoints.Length); i++)
        {
            var randomPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
            var enemy = Instantiate(enemyEntityPrefab, randomPoint.position, Quaternion.identity);
            enemy.UnicId = Guid.NewGuid();
            enemy.OnDie += OnEnemyDie;
            Wave.Add(enemy);
        }
    }

    private void OnEnemyDie(Guid enemyId)
    {
        var deadEnemy = Wave.First(x => x.UnicId == enemyId);
        deadEnemy.OnDie -= OnEnemyDie; 
        Wave.Remove(deadEnemy);
    }
}
