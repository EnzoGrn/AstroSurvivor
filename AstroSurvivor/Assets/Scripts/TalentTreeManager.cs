using System;
using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// Gère l'interaction et la progression dans l'arbre de talents
    /// </summary>
    public class TalentTreeManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private TalentTree talentTree;
        [SerializeField] private PlayerStats playerStats;

        [Header("Progression")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int availableTalentPoints = 0;
        [SerializeField] private int totalPointsSpent = 0;

        // Talents débloqués
        private Dictionary<string, UnlockedTalent> unlockedTalents = new Dictionary<string, UnlockedTalent>();
        private HashSet<string> unlockedNodeIds = new HashSet<string>();

        // Events
        public event Action<TalentNodeData, int> OnTalentUnlocked; // talent, points investis
        public event Action<int> OnTalentPointsChanged; // nouveaux points disponibles
        public event Action<int> OnLevelUp; // nouveau niveau
        public event Action OnTalentTreeReset;

        #region Properties
        public TalentTree TalentTree => talentTree;
        public int AvailableTalentPoints => availableTalentPoints;
        public int TotalPointsSpent => totalPointsSpent;
        public int CurrentLevel => currentLevel;
        public Dictionary<string, UnlockedTalent> UnlockedTalents => unlockedTalents;
        #endregion

        private void Awake()
        {
            if (playerStats == null)
            {
                playerStats = GetComponent<PlayerStats>();
            }

            if (playerStats == null)
            {
                Debug.LogError("PlayerStats non trouvé! Assurez-vous que TalentTreeManager et PlayerStats sont sur le même GameObject ou assignez PlayerStats manuellement.");
            }
        }

        private void Start()
        {
            if (talentTree != null)
            {
                // Valide l'arbre au démarrage (en mode debug)
                #if UNITY_EDITOR
                if (!talentTree.ValidateTree())
                {
                    Debug.LogWarning("L'arbre de talents contient des erreurs!");
                }
                #endif
            }
            else
            {
                Debug.LogWarning("Aucun TalentTree assigné au TalentTreeManager!");
            }
        }

        /// <summary>
        /// Gagne un niveau et des points de talents
        /// </summary>
        public void LevelUp()
        {
            currentLevel++;
            
            if (talentTree != null)
            {
                AddTalentPoints(talentTree.pointsPerLevel);
            }

            OnLevelUp?.Invoke(currentLevel);
            Debug.Log($"Level Up! Niveau {currentLevel}. Points de talent: +{talentTree.pointsPerLevel}");
        }

        /// <summary>
        /// Ajoute des points de talents
        /// </summary>
        public void AddTalentPoints(int points)
        {
            availableTalentPoints += points;
            OnTalentPointsChanged?.Invoke(availableTalentPoints);
        }

        /// <summary>
        /// Tente de débloquer un talent
        /// </summary>
        public bool TryUnlockTalent(string nodeId)
        {
            if (talentTree == null)
            {
                Debug.LogError("Aucun TalentTree assigné!");
                return false;
            }

            TalentNodeData node = talentTree.GetNodeById(nodeId);
            
            if (node == null)
            {
                Debug.LogError($"Noeud avec l'ID '{nodeId}' introuvable!");
                return false;
            }

            // Vérifie si on peut débloquer ce talent
            if (!CanUnlockTalent(node))
            {
                Debug.Log($"Impossible de débloquer '{node.talentName}': conditions non remplies");
                return false;
            }

            // Calcule combien de points investir
            int currentPoints = 0;
            if (unlockedTalents.ContainsKey(nodeId))
            {
                currentPoints = unlockedTalents[nodeId].pointsInvested;
                
                // Vérifie si on peut encore investir des points
                if (currentPoints >= node.maxPoints)
                {
                    Debug.Log($"'{node.talentName}' est déjà au maximum ({node.maxPoints} points)");
                    return false;
                }
            }

            // Vérifie le coût
            if (availableTalentPoints < node.pointCost)
            {
                Debug.Log($"Points de talents insuffisants! Requis: {node.pointCost}, Disponibles: {availableTalentPoints}");
                return false;
            }

            // Débloque le talent
            UnlockTalent(node);
            return true;
        }

        private void UnlockTalent(TalentNodeData node)
        {
            int pointsToInvest = 1;

            // Si le talent existe déjà, on ajoute juste un point
            if (unlockedTalents.ContainsKey(node.nodeId))
            {
                unlockedTalents[node.nodeId].pointsInvested++;
                pointsToInvest = 1;
            }
            else
            {
                // Nouveau talent
                UnlockedTalent newTalent = new UnlockedTalent(node.nodeId, node, 1);
                unlockedTalents.Add(node.nodeId, newTalent);
                unlockedNodeIds.Add(node.nodeId);
            }

            // Applique les modificateurs
            node.ApplyModifiers(playerStats, pointsToInvest);

            // Dépense les points
            availableTalentPoints -= node.pointCost;
            totalPointsSpent += node.pointCost;

            // Notifications
            OnTalentUnlocked?.Invoke(node, unlockedTalents[node.nodeId].pointsInvested);
            OnTalentPointsChanged?.Invoke(availableTalentPoints);

            Debug.Log($"Talent '{node.talentName}' débloqué! Points investis: {unlockedTalents[node.nodeId].pointsInvested}/{node.maxPoints}");
        }

        /// <summary>
        /// Vérifie si un talent peut être débloqué
        /// </summary>
        public bool CanUnlockTalent(TalentNodeData node)
        {
            if (node == null) return false;

            // Vérifie les prérequis
            if (!node.CanBeUnlocked(unlockedNodeIds, currentLevel))
            {
                return false;
            }

            // Vérifie si déjà au maximum
            if (unlockedTalents.ContainsKey(node.nodeId))
            {
                if (unlockedTalents[node.nodeId].pointsInvested >= node.maxPoints)
                {
                    return false;
                }
            }

            // Vérifie les points disponibles
            if (availableTalentPoints < node.pointCost)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Vérifie si un talent est débloqué
        /// </summary>
        public bool IsTalentUnlocked(string nodeId)
        {
            return unlockedNodeIds.Contains(nodeId);
        }

        /// <summary>
        /// Obtient le nombre de points investis dans un talent
        /// </summary>
        public int GetTalentPoints(string nodeId)
        {
            if (unlockedTalents.ContainsKey(nodeId))
            {
                return unlockedTalents[nodeId].pointsInvested;
            }
            return 0;
        }

        /// <summary>
        /// Réinitialise tous les talents (coûte peut-être de l'argent en jeu)
        /// </summary>
        public void ResetTalents()
        {
            // Redonne tous les points dépensés
            availableTalentPoints += totalPointsSpent;
            totalPointsSpent = 0;

            // Efface les talents
            unlockedTalents.Clear();
            unlockedNodeIds.Clear();

            // Réinitialise les stats du joueur
            if (playerStats != null)
            {
                playerStats.InitializeStats();
            }

            OnTalentTreeReset?.Invoke();
            OnTalentPointsChanged?.Invoke(availableTalentPoints);

            Debug.Log("Arbre de talents réinitialisé!");
        }

        #region Sauvegarde/Chargement
        /// <summary>
        /// Sauvegarde les données de progression des talents
        /// </summary>
        [System.Serializable]
        public class TalentSaveData
        {
            public int level;
            public int availablePoints;
            public int totalSpent;
            public List<UnlockedTalentSave> unlockedTalents;

            [System.Serializable]
            public class UnlockedTalentSave
            {
                public string nodeId;
                public int points;
            }
        }

        public TalentSaveData GetSaveData()
        {
            TalentSaveData saveData = new TalentSaveData
            {
                level = currentLevel,
                availablePoints = availableTalentPoints,
                totalSpent = totalPointsSpent,
                unlockedTalents = new List<TalentSaveData.UnlockedTalentSave>()
            };

            foreach (var talent in unlockedTalents.Values)
            {
                saveData.unlockedTalents.Add(new TalentSaveData.UnlockedTalentSave
                {
                    nodeId = talent.nodeId,
                    points = talent.pointsInvested
                });
            }

            return saveData;
        }

        public void LoadSaveData(TalentSaveData saveData)
        {
            if (saveData == null || talentTree == null) return;

            // Réinitialise d'abord
            ResetTalents();

            // Charge les données
            currentLevel = saveData.level;
            availableTalentPoints = saveData.availablePoints;
            totalPointsSpent = saveData.totalSpent;

            // Recharge les talents
            foreach (var talentSave in saveData.unlockedTalents)
            {
                TalentNodeData node = talentTree.GetNodeById(talentSave.nodeId);
                if (node != null)
                {
                    UnlockedTalent talent = new UnlockedTalent(talentSave.nodeId, node, talentSave.points);
                    unlockedTalents.Add(talentSave.nodeId, talent);
                    unlockedNodeIds.Add(talentSave.nodeId);

                    // Réapplique les modificateurs
                    node.ApplyModifiers(playerStats, talentSave.points);
                }
            }

            Debug.Log("Talents chargés avec succès!");
        }
        #endregion

        #region Debug
        [ContextMenu("Add 10 Talent Points")]
        private void Debug_Add10Points()
        {
            AddTalentPoints(10);
        }

        [ContextMenu("Level Up")]
        private void Debug_LevelUp()
        {
            LevelUp();
        }

        [ContextMenu("Reset Talents")]
        private void Debug_ResetTalents()
        {
            ResetTalents();
        }
        #endregion
    }
}
