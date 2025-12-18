using UnityEngine;

public class EnemyDeathExplosion : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    public void Play()
    {
        if (!explosionPrefab)
            return;

        Instantiate(
            explosionPrefab,
            transform.position,
            Quaternion.identity
        );
    }
}
