using AstroSurvivor;
using AstroSurvivor.UI;
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

    public WaveUI waveUI;
    public UpgradeUIController upgradePanelController;
    public TalentTreeManager TalentTree;

    private bool _Selected;

    private void Start()
    {
        StartCoroutine(InfiniteWaveLoop());
    }

    private IEnumerator InfiniteWaveLoop()
    {
        while (true) {
            _Selected = false;

            waveUI.ShowWave(currentWave);

            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(SpawnProceduralWave());
            yield return new WaitUntil(() => aliveEnemies == 0);

            if (upgradePanelController.Open()) {
                upgradePanelController.OnSelected += OnNextPhase;

                yield return new WaitUntil(() => _Selected == true);
            }

            TalentTree.LevelUp();

            currentWave++;
            currentZone++;
        }
    }

    public void OnNextPhase()
    {
        upgradePanelController.OnSelected -= OnNextPhase;

        _Selected = true;
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
        int maxIndex = Mathf.Min(enemyPool.Length, 1 + currentWave / 2);

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
