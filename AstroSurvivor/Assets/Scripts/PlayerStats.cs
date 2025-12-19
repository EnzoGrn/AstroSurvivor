using System;
using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// Gère toutes les statistiques du joueur
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Points de vie")]
        [SerializeField] private float baseMaxHp = 100f;
        [SerializeField] private float baseShield = 0f;

        [Header("Dégâts")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float baseCriticalChance = 5f; // En pourcentage
        [SerializeField] private float baseCriticalDamage = 150f; // En pourcentage (150% = 1.5x)

        [Header("Attaque")]
        [SerializeField] private float baseAttackSpeed = 1f; // Attaques par seconde
        [SerializeField] private int baseProjectileCount = 1;
        [SerializeField] private float baseRange = 10f;

        [Header("Movement")]
        [SerializeField] private float baseMoveSpeed = 20f;

        // Valeurs actuelles (avec modificateurs)
        private float currentMaxHp;
        private float currentHp;
        private float currentShield;
        public float currentDamage;
        private float currentCriticalChance;
        private float currentCriticalDamage;
        private float currentAttackSpeed;
        private int currentProjectileCount;
        private float currentRange;

        // Modificateurs en pourcentage
        private float maxHpModifier = 0f;
        private float damageModifier = 0f;
        private float criticalChanceModifier = 0f;
        private float criticalDamageModifier = 0f;
        private float attackSpeedModifier = 0f;
        private int projectileCountModifier = 0;
        private float rangeModifier = 0f;
        public float moveSpeedModifier = 0f;

        // Events pour notifier les changements
        public event Action<float, float> OnHealthChanged; // currentHp, maxHp
        public event Action<float> OnShieldChanged;
        public event Action OnStatsChanged;
        public event Action OnPlayerDied;

        #region Properties
        public float MaxHp => currentMaxHp;
        public float CurrentHp => currentHp;
        public float Shield => currentShield;
        public float Damage => currentDamage;
        public float CriticalChance => currentCriticalChance;
        public float CriticalDamage => currentCriticalDamage;
        public float AttackSpeed => currentAttackSpeed;
        public int ProjectileCount => currentProjectileCount;
        public float Range => currentRange;
        public bool IsAlive => currentHp > 0;

        public float MoveSpeed => baseMoveSpeed * (1f + moveSpeedModifier / 100f);

        #endregion

        private void Start()
        {
            InitializeStats();
        }

        /// <summary>
        /// Initialise toutes les statistiques aux valeurs de base
        /// </summary>
        public void InitializeStats()
        {
            RecalculateStats();
            currentHp = currentMaxHp;
            currentShield = baseShield;
            OnHealthChanged?.Invoke(currentHp, currentMaxHp);
            OnShieldChanged?.Invoke(currentShield);
        }

        /// <summary>
        /// Recalcule toutes les stats en fonction des modificateurs
        /// </summary>
        public void RecalculateStats()
        {
            currentMaxHp = baseMaxHp * (1f + maxHpModifier / 100f);
            currentDamage = baseDamage * (1f + damageModifier / 100f);
            currentCriticalChance = baseCriticalChance + criticalChanceModifier;
            currentCriticalDamage = baseCriticalDamage + criticalDamageModifier;
            currentAttackSpeed = baseAttackSpeed * (1f + attackSpeedModifier / 100f);
            currentProjectileCount = baseProjectileCount + projectileCountModifier;
            currentRange = baseRange * (1f + rangeModifier / 100f);

            // Clamp les valeurs si nécessaire
            currentCriticalChance = Mathf.Clamp(currentCriticalChance, 0f, 100f);
            currentProjectileCount = Mathf.Max(1, currentProjectileCount);

            OnStatsChanged?.Invoke();
        }

        #region Modificateurs de stats
        /// <summary>
        /// Ajoute un modificateur de HP max en pourcentage
        /// </summary>
        public void AddMaxHpModifier(float percentModifier)
        {
            maxHpModifier += percentModifier;
            float hpPercentage = currentHp / currentMaxHp;
            RecalculateStats();
            currentHp = currentMaxHp * hpPercentage; // Garde le même pourcentage de HP
            OnHealthChanged?.Invoke(currentHp, currentMaxHp);
        }

        /// <summary>
        /// Ajoute un modificateur de dégâts en pourcentage
        /// </summary>
        public void AddDamageModifier(float percentModifier)
        {
            damageModifier += percentModifier;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute un modificateur de chance critique (valeur brute en %)
        /// </summary>
        public void AddCriticalChanceModifier(float percentValue)
        {
            criticalChanceModifier += percentValue;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute un modificateur de dégâts critiques (valeur brute en %)
        /// </summary>
        public void AddCriticalDamageModifier(float percentValue)
        {
            criticalDamageModifier += percentValue;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute un modificateur de vitesse d'attaque en pourcentage
        /// </summary>
        public void AddAttackSpeedModifier(float percentModifier)
        {
            attackSpeedModifier += percentModifier;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute des projectiles supplémentaires
        /// </summary>
        public void AddProjectileCount(int count)
        {
            projectileCountModifier += count;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute un modificateur de portée en pourcentage
        /// </summary>
        public void AddRangeModifier(float percentModifier)
        {
            rangeModifier += percentModifier;
            RecalculateStats();
        }

        /// <summary>
        /// Ajoute du bouclier
        /// </summary>
        public void AddShield(float amount)
        {
            currentShield += amount;
            OnShieldChanged?.Invoke(currentShield);
        }
        #endregion

        #region Gestion des dégâts et soins
        /// <summary>
        /// Inflige des dégâts au joueur (prend en compte le bouclier)
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (!IsAlive) return;

            // Le bouclier absorbe d'abord les dégâts
            if (currentShield > 0)
            {
                float remainingDamage = damage - currentShield;
                currentShield = Mathf.Max(0, currentShield - damage);
                OnShieldChanged?.Invoke(currentShield);

                if (remainingDamage > 0)
                {
                    currentHp = Mathf.Max(0, currentHp - remainingDamage);
                }
            }
            else
            {
                currentHp = Mathf.Max(0, currentHp - damage);
            }

            OnHealthChanged?.Invoke(currentHp, currentMaxHp);

            if (currentHp <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Soigne le joueur
        /// </summary>
        public void Heal(float amount)
        {
            if (!IsAlive) return;

            currentHp = Mathf.Min(currentMaxHp, currentHp + amount);
            OnHealthChanged?.Invoke(currentHp, currentMaxHp);
        }

        /// <summary>
        /// Soigne le joueur avec un pourcentage de ses HP max
        /// </summary>
        public void HealPercent(float percent)
        {
            Heal(currentMaxHp * (percent / 100f));
        }

        /// <summary>
        /// Calcule les dégâts avec la chance de critique
        /// </summary>
        public float CalculateDamage()
        {
            bool isCritical = UnityEngine.Random.Range(0f, 100f) < currentCriticalChance;

            if (isCritical)
                return currentDamage * (currentCriticalDamage / 100f);
            return currentDamage;
        }

        private void Die()
        {
            OnPlayerDied?.Invoke();
            Debug.Log("Player died!");
        }
        #endregion

        #region Debug

        private void OnEnable()
        {
            OnStatsChanged += Debug_PrintStats;
        }

        private void Debug_PrintStats()
        {
            Debug.Log(
                $"HP:{currentMaxHp} | DMG:{currentDamage} | " +
                $"CRIT:{currentCriticalChance}% | AS:{currentAttackSpeed} | " +
                $"PROJ:{currentProjectileCount}"
            );
        }

        private void OnValidate()
        {
            // Validation en mode éditeur
            baseMaxHp = Mathf.Max(1f, baseMaxHp);
            baseDamage = Mathf.Max(0f, baseDamage);
            baseCriticalChance = Mathf.Clamp(baseCriticalChance, 0f, 100f);
            baseCriticalDamage = Mathf.Max(0f, baseCriticalDamage);
            baseAttackSpeed = Mathf.Max(0.1f, baseAttackSpeed);
            baseProjectileCount = Mathf.Max(1, baseProjectileCount);
            baseRange = Mathf.Max(0f, baseRange);
        }
        #endregion
    }
}
