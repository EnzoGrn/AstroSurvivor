using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyWaveSO[] waves;
    public Transform[] spawnPoints;
    public int currentZone = 1;

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        foreach (var wave in waves)
        {
            yield return new WaitForSeconds(wave.spawnDelay);

            foreach (var enemyData in wave.enemiesInWave)
            {
                for (int i = 0; i < enemyData.count; i++)
                {
                    SpawnEnemy(enemyData.enemy);
                }
            }
        }
    }

    private void SpawnEnemy(EnemySO enemySO)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyGO = Instantiate(enemySO.prefab, spawnPoint.position, Quaternion.identity);

        Enemy enemyComponent = enemyGO.GetComponent<Enemy>();
        enemyComponent.Setup(enemySO, currentZone);
    }

}
