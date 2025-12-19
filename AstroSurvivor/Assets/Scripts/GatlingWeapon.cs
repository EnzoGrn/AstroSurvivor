using AstroSurvivor;
using UnityEngine;

public class GatlingWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;
    [SerializeField] private int projectileDamage = 1;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 30;

    private float _fireTimer;
    private Projectile[] _projectilePool;
    private int _currentIndex;

    private PlayerStats _stats;

    private void Awake()
    {
        _stats = GetComponentInParent<PlayerStats>();

        InitializePool();
    }

    private void Update()
    {
        HandleAutoFire();
    }

    private void HandleAutoFire()
    {
        float fireRate = _stats.AttackSpeed;

        _fireTimer += Time.deltaTime;

        if (_fireTimer >= 1f / fireRate) {
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
            (int)_stats.CalculateDamage()
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
