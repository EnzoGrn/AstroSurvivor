using UnityEngine;

public class GatlingWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float fireRate = 10f;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private int projectileDamage = 1;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 30;

    private float _fireTimer;
    private Projectile[] _projectilePool;
    private int _currentIndex;

    private void Awake()
    {
        InitializePool();
    }

    private void Update()
    {
        HandleAutoFire();
    }

    private void HandleAutoFire()
    {
        _fireTimer += Time.deltaTime;

        if (_fireTimer >= 1f / fireRate)
        {
            Fire();
            _fireTimer = 0f;
        }
    }

    private void Fire()
    {
        Projectile projectile = GetNextProjectile();

        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;

        projectile.Fire(
            transform.forward,
            projectileSpeed,
            projectileDamage
        );
    }

    private void InitializePool()
    {
        _projectilePool = new Projectile[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            Projectile proj = Instantiate(projectilePrefab);
            proj.gameObject.SetActive(false);
            _projectilePool[i] = proj;
        }
    }

    private Projectile GetNextProjectile()
    {
        Projectile proj = _projectilePool[_currentIndex];
        _currentIndex = (_currentIndex + 1) % poolSize;
        proj.gameObject.SetActive(true);
        return proj;
    }
}
