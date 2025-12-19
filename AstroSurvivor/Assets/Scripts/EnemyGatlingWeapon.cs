using AstroSurvivor;
using UnityEngine;

public class EnemyGatlingWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private float projectileSpeed = 25f;

    [Header("Pooling")]
    [SerializeField] private int poolSize = 30;

    private float _fireTimer;
    private EnemyProjectile[] _projectilePool;
    private int _currentIndex;

    private PlayerStats _stats;
    [SerializeField] private EnemySO enemySO;

    [SerializeField] private bool _isFiring = false;

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
        if (!_isFiring) return;

        float fireRate = _stats.AttackSpeed;

        _fireTimer += Time.deltaTime;

        if (_fireTimer >= 1f / fireRate) {
            Fire();

            _fireTimer = 0f;
        }
    }

    public void Fire()
    {
        EnemyProjectile projectile = GetNextProjectile();

        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;

        projectile.Fire(
            transform.forward,
            projectileSpeed,
            _stats ? (int)_stats.CalculateDamage() : enemySO.baseDamage
        );
    }

    private void InitializePool()
    {
        _projectilePool = new EnemyProjectile[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            EnemyProjectile proj = Instantiate(projectilePrefab);
            proj.gameObject.SetActive(false);
            _projectilePool[i] = proj;
        }
    }

    private EnemyProjectile GetNextProjectile()
    {
        EnemyProjectile proj = _projectilePool[_currentIndex];
        _currentIndex = (_currentIndex + 1) % poolSize;
        proj.gameObject.SetActive(true);
        return proj;
    }

    public void StartFiring()
    {
        _isFiring = true;
    }

    public void StopFiring()
    {
        _isFiring = false;
    }
}
