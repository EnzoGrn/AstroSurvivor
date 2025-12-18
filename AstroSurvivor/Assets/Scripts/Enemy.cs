using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player;

    private int hp;
    private int damage;
    private float speed;
    private float attackRange;
    private float attackSpeed;
    private EnemyAttackType attackType;

    private float attackTimer;

    public void Setup(EnemySO so, int zone)
    {
        hp = so.GetScaledHP(zone);
        damage = so.GetScaledDamage(zone);
        speed = so.speed;
        attackRange = so.attackRange;
        attackSpeed = so.attackSpeed;
        attackType = so.attackType;

        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                Time.deltaTime * 10f
            );
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
                // dégâts au contact (via collider)
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
        Debug.Log("Gatling shot");
        // spawn projectile ici
    }

    private void LaunchMissile()
    {
        Debug.Log("Missile launched");
        // spawn missile ici
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Destroy(gameObject);
    }
}
