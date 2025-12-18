using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    [Header("Enemies")]
    public EnemySO[] enemyPool;
    public Transform[] spawnPoints;

    [Header("Difficulty")]
    public int currentWave = 1;
    public int currentZone = 1;

    public int baseEnemyCount = 5;
    public float baseSpawnInterval = 0.4f;

    private int aliveEnemies = 0;

    private void Start()
    {
        StartCoroutine(InfiniteWaveLoop());
    }

    private IEnumerator InfiniteWaveLoop()
    {
        while (true) {

            yield return StartCoroutine(SpawnProceduralWave());
            yield return new WaitUntil(() => aliveEnemies == 0);

            currentWave++;
            currentZone++;

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator SpawnProceduralWave()
    {
        int enemyCount = GetEnemyCount();

        float spawnInterval = GetSpawnInterval();

        for (int i = 0; i < enemyCount; i++) {
            SpawnEnemy(SelectEnemy());

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int GetEnemyCount()
    {
        return Mathf.RoundToInt(baseEnemyCount * (1f + currentWave * 0.25f));
    }

    private float GetSpawnInterval()
    {
        return Mathf.Max(0.1f, baseSpawnInterval - currentWave * 0.02f);
    }

    private EnemySO SelectEnemy()
    {
        int maxIndex = Mathf.Min(enemyPool.Length, 1 + currentWave / 5);

        return enemyPool[Random.Range(0, maxIndex)];
    }

    private void SpawnEnemy(EnemySO enemySO)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemySO.prefab, spawnPoint.position, Quaternion.identity);

        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.Setup(enemySO, currentZone);

        aliveEnemies++;

        enemy.OnDeath += OnEnemyKilled;
    }

    private void OnEnemyKilled(Enemy enemy)
    {
        enemy.OnDeath -= OnEnemyKilled;

        aliveEnemies--;
    }
}
