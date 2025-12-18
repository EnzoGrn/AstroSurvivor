using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public EnemySO enemy;
    public int count;
}

[CreateAssetMenu(fileName = "EnemyWaveSO", menuName = "AstroSurvivor/Enemies/Wave")]
public class EnemyWaveSO : ScriptableObject
{
    public EnemySpawnData[] enemiesInWave;
    public float spawnDelay;
}
