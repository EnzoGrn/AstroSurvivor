using UnityEngine;

public enum EnemyAttackType
{
    None,
    Ram,
    Gatling,
    Missile
}

[CreateAssetMenu(menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    public GameObject prefab;

    public int baseHP;
    public int baseDamage;
    public float speed;

    public EnemyAttackType attackType;
    public float attackRange;
    public float attackSpeed; // tirs par seconde

    public int GetScaledHP(int zone)
        => Mathf.RoundToInt(baseHP * Mathf.Pow(1.2f, zone - 1));

    public int GetScaledDamage(int zone)
        => Mathf.RoundToInt(baseDamage * Mathf.Pow(1.2f, zone - 1));
}
