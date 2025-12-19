using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// ScriptableObject représentant un noeud de l'arbre de talents
    /// </summary>
    [CreateAssetMenu(fileName = "NewTalentNode", menuName = "AstroSurvivor/Talent Node")]
    public class TalentNodeData : ScriptableObject
    {
        [Header("Informations du Talent")]
        [Tooltip("Nom du talent")]
        public string talentName;

        [Tooltip("Description du talent")]
        [TextArea(3, 5)]
        public string description;

        [Tooltip("Icône du talent")]
        public Sprite icon;

        [Header("Modificateurs de Stats")]
        [Tooltip("Liste des modificateurs de stats que ce talent applique")]
        public List<StatModifier> statModifiers = new List<StatModifier>();

        [Header("Structure de l'Arbre")]
        [Tooltip("ID unique du noeud")]
        public string nodeId;

        [Tooltip("Niveau requis dans l'arbre pour débloquer ce talent")]
        public int requiredLevel = 1;

        [Tooltip("IDs des noeuds parents requis (au moins un doit être débloqué)")]
        public List<string> parentNodeIds = new List<string>();

        [Tooltip("Nombre maximum de points pouvant être investis dans ce talent")]
        public int maxPoints = 1;

        [Header("Coût")]
        [Tooltip("Coût en points de talent pour débloquer")]
        public int pointCost = 1;

        [Header("Visuels")]
        [Tooltip("Position du noeud dans l'arbre (pour l'affichage UI)")]
        public Vector2 treePosition;

        [Tooltip("Couleur du noeud (utilisé pour catégoriser visuellement)")]
        public Color nodeColor = Color.white;

        /// <summary>
        /// Applique tous les modificateurs de ce talent au joueur
        /// </summary>
        public void ApplyModifiers(PlayerStats playerStats, int points = 1)
        {
            if (playerStats == null)
            {
                Debug.LogError($"PlayerStats est null lors de l'application du talent {talentName}");
                return;
            }

            foreach (var modifier in statModifiers)
            {
                // Multiplie la valeur du modificateur par le nombre de points investis
                StatModifier scaledModifier = new StatModifier(
                    modifier.statType,
                    modifier.value * points,
                    modifier.description
                );
                scaledModifier.ApplyToPlayer(playerStats);
            }

            Debug.Log($"Talent '{talentName}' appliqué avec {points} point(s)");
        }

        /// <summary>
        /// Vérifie si ce noeud peut être débloqué
        /// </summary>
        public bool CanBeUnlocked(HashSet<string> unlockedNodeIds, int currentTreeLevel)
        {
            // Vérifie le niveau requis
            if (currentTreeLevel < requiredLevel)
            {
                return false;
            }

            // Si pas de parents requis, peut être débloqué
            if (parentNodeIds.Count == 0)
            {
                return true;
            }

            // Vérifie qu'au moins un parent est débloqué
            foreach (string parentId in parentNodeIds)
            {
                if (unlockedNodeIds.Contains(parentId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Retourne une description complète avec tous les modificateurs
        /// </summary>
        public string GetFullDescription()
        {
            string fullDesc = description;

            if (statModifiers.Count > 0)
            {
                fullDesc += "<b>Bonus:</b>";
                foreach (var modifier in statModifiers)
                {
                    fullDesc += $"\n• {modifier.GetFormattedDescription()}";
                }
            }

            if (maxPoints > 1)
            {
                fullDesc += $"\n<i>Points max: {maxPoints}</i>";
            }

            return fullDesc;
        }

        private void OnValidate()
        {
            // Validation en mode éditeur
            requiredLevel = Mathf.Max(1, requiredLevel);
            maxPoints = Mathf.Max(1, maxPoints);
            pointCost = Mathf.Max(1, pointCost);

            // Génère un ID si vide
            if (string.IsNullOrEmpty(nodeId))
            {
                nodeId = System.Guid.NewGuid().ToString();
            }
        }
    }
}
