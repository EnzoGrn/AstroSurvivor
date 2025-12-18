using System;
using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// Type de statistique à modifier
    /// </summary>
    public enum StatType
    {
        MaxHp,
        Damage,
        CriticalChance,
        CriticalDamage,
        AttackSpeed,
        ProjectileCount,
        Range,
        Shield
    }

    /// <summary>
    /// Représente un modificateur de statistique
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        [Tooltip("Type de statistique à modifier")]
        public StatType statType;

        [Tooltip("Valeur du modificateur")]
        public float value;

        [Tooltip("Description du modificateur")]
        public string description;

        public StatModifier(StatType type, float val, string desc = "")
        {
            statType = type;
            value = val;
            description = desc;
        }

        /// <summary>
        /// Applique ce modificateur aux stats du joueur
        /// </summary>
        public void ApplyToPlayer(PlayerStats playerStats)
        {
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats est null, impossible d'appliquer le modificateur!");
                return;
            }

            switch (statType)
            {
                case StatType.MaxHp:
                    playerStats.AddMaxHpModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% HP Max");
                    break;

                case StatType.Damage:
                    playerStats.AddDamageModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% Dégâts");
                    break;

                case StatType.CriticalChance:
                    playerStats.AddCriticalChanceModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% Chance Critique");
                    break;

                case StatType.CriticalDamage:
                    playerStats.AddCriticalDamageModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% Dégâts Critiques");
                    break;

                case StatType.AttackSpeed:
                    playerStats.AddAttackSpeedModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% Vitesse d'Attaque");
                    break;

                case StatType.ProjectileCount:
                    playerStats.AddProjectileCount((int)value);
                    Debug.Log($"Modificateur appliqué: +{(int)value} Projectiles");
                    break;

                case StatType.Range:
                    playerStats.AddRangeModifier(value);
                    Debug.Log($"Modificateur appliqué: +{value}% Portée");
                    break;

                case StatType.Shield:
                    playerStats.AddShield(value);
                    Debug.Log($"Modificateur appliqué: +{value} Bouclier");
                    break;

                default:
                    Debug.LogWarning($"Type de stat non géré: {statType}");
                    break;
            }
        }

        /// <summary>
        /// Retourne une description formatée du modificateur
        /// </summary>
        public string GetFormattedDescription()
        {
            string sign = value >= 0 ? "+" : "";
            
            switch (statType)
            {
                case StatType.ProjectileCount:
                    return $"{sign}{(int)value} {GetStatName()}";
                case StatType.Shield:
                    return $"{sign}{value} {GetStatName()}";
                default:
                    return $"{sign}{value}% {GetStatName()}";
            }
        }

        /// <summary>
        /// Retourne le nom de la statistique
        /// </summary>
        public string GetStatName()
        {
            switch (statType)
            {
                case StatType.MaxHp: return "HP Max";
                case StatType.Damage: return "Dégâts";
                case StatType.CriticalChance: return "Chance Critique";
                case StatType.CriticalDamage: return "Dégâts Critiques";
                case StatType.AttackSpeed: return "Vitesse d'Attaque";
                case StatType.ProjectileCount: return "Projectiles";
                case StatType.Range: return "Portée";
                case StatType.Shield: return "Bouclier";
                default: return "Stat Inconnue";
            }
        }
    }
}
