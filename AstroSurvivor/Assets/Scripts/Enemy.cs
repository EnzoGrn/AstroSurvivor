using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Transform player;

    private int _currentHealth;
    private int _maxHealth;
    private int damage;
    private float speed;
    private float attackRange;
    private float attackSpeed;
    private EnemyAttackType attackType;

    private float attackTimer;

    private EnemyFeedback _feedback;
    private EnemyDeathExplosion _explosion;

    public Action<Enemy> OnDeath;

    // ======================
    // HEALTH BAR
    // ======================
    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0f, 2f, 0f);

    private Image _healthFill;
    private Transform _healthBarTransform;
    private Camera _cam;

    public void Setup(EnemySO so, int zone)
    {
        _maxHealth = so.GetScaledHP(zone);
        _currentHealth = _maxHealth;
        damage = so.GetScaledDamage(zone);
        speed = so.speed;
        attackRange = so.attackRange;
        attackSpeed = so.attackSpeed;
        attackType = so.attackType;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        _feedback = GetComponent<EnemyFeedback>();
        _explosion = GetComponent<EnemyDeathExplosion>();

        SetupHealthBar();
    }

    private void SetupHealthBar()
    {
        if (healthBarPrefab == null) return;

        GameObject hb = Instantiate(healthBarPrefab, transform);
        hb.transform.localPosition = healthBarOffset;

        _healthBarTransform = hb.transform;
        _healthFill = hb.GetComponentInChildren<Image>();
        _cam = Camera.main;

        UpdateHealthBar();
    }

    private void Update()
    {
        if (!player) return;

        MoveTowardPlayer();
        RotateTowardPlayer();
        HandleAttack();
        UpdateHealthBarRotation();
    }

    private void UpdateHealthBarRotation()
    {
        if (_healthBarTransform != null && _cam != null)
        {
            _healthBarTransform.rotation =
                Quaternion.LookRotation(_healthBarTransform.position - _cam.transform.position);
        }
    }

    private void UpdateHealthBar()
    {
        if (_healthFill != null)
        {
            float percent = (float)_currentHealth / _maxHealth;
            _healthFill.fillAmount = percent;
        }
    }

    private void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void RotateTowardPlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10f
            );
        }
    }

    private void HandleAttack()
    {
        if (attackType == EnemyAttackType.None) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackRange) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= 1f / attackSpeed)
        {
            attackTimer = 0f;
            Attack();
        }
    }

    private void Attack()
    {
        switch (attackType)
        {
            case EnemyAttackType.Ram:
                break;
            case EnemyAttackType.Gatling:
                Shoot();
                break;
            case EnemyAttackType.Missile:
                LaunchMissile();
                break;
        }
    }

    private void Shoot()
    {
        Debug.Log("Enemy Gatling shot");
    }

    private void LaunchMissile()
    {
        Debug.Log("Enemy Missile launched");
    }

    // ======================
    // DAMAGE & FEEDBACK
    // ======================

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        _feedback?.OnHit();

        UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _feedback?.OnDeath();
        _explosion?.Play();

        OnDeath?.Invoke(this);

        if (_healthBarTransform != null)
            Destroy(_healthBarTransform.gameObject);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    // ======================
    // RAM ATTACK
    // ======================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerController3D playerController))
        {
            playerController.TakeDamage(damage);

            PlayerFeedback playerFeedback = playerController.GetComponent<PlayerFeedback>();
            playerFeedback?.OnPlayerHit();

            OnRamHit();
        }
    }

    private void OnRamHit()
    {
        Die();
    }
}
