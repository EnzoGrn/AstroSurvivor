using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    private Transform player;

    private int _currentHealth;
    private int damage;
    private float speed;
    private float attackRange;
    private float attackSpeed;
    private EnemyAttackType attackType;

    private float attackTimer;

    private EnemyFeedback _feedback;
    private EnemyDeathExplosion _explosion;

    public void Setup(EnemySO so, int zone)
    {
        _currentHealth = so.GetScaledHP(zone);
        damage = so.GetScaledDamage(zone);
        speed = so.speed;
        attackRange = so.attackRange;
        attackSpeed = so.attackSpeed;
        attackType = so.attackType;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        _feedback = GetComponent<EnemyFeedback>();
        _explosion = GetComponent<EnemyDeathExplosion>();
    }

    private void Update()
    {
        if (!player) return;

        MoveTowardPlayer();
        RotateTowardPlayer();
        HandleAttack();
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
                // handled by collision
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
        // TODO : spawn projectile
        Debug.Log("Enemy Gatling shot");
    }

    private void LaunchMissile()
    {
        // TODO : spawn missile
        Debug.Log("Enemy Missile launched");
    }

    // ======================
    // DAMAGE & FEEDBACK
    // ======================

    public void TakeDamage(int dmg)
    {
        _currentHealth -= dmg;
        _feedback?.OnHit();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _feedback?.OnDeath();
        _explosion?.Play();

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
