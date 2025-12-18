using UnityEngine;

namespace AstroSurvivor
{
    /// <summary>
    /// Exemple d'utilisation du système de stats et de talents
    /// Ce script montre comment configurer et utiliser PlayerStats et TalentTreeManager
    /// </summary>
    public class ExampleUsage : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private TalentTreeManager talentTreeManager;

        private void Start()
        {
            SetupEventListeners();
            
            // Exemple: Donner quelques points de talents au départ
            talentTreeManager.AddTalentPoints(5);
        }

        /// <summary>
        /// Configure les écouteurs d'événements
        /// </summary>
        private void SetupEventListeners()
        {
            // Events des stats du joueur
            if (playerStats != null)
            {
                playerStats.OnHealthChanged += HandleHealthChanged;
                playerStats.OnShieldChanged += HandleShieldChanged;
                playerStats.OnStatsChanged += HandleStatsChanged;
                playerStats.OnPlayerDied += HandlePlayerDied;
            }

            // Events de l'arbre de talents
            if (talentTreeManager != null)
            {
                talentTreeManager.OnTalentUnlocked += HandleTalentUnlocked;
                talentTreeManager.OnTalentPointsChanged += HandleTalentPointsChanged;
                talentTreeManager.OnLevelUp += HandleLevelUp;
                talentTreeManager.OnTalentTreeReset += HandleTalentTreeReset;
            }
        }

        #region Handlers d'Events - Stats
        private void HandleHealthChanged(float currentHp, float maxHp)
        {
            Debug.Log($"HP: {currentHp}/{maxHp} ({(currentHp / maxHp * 100f):F1}%)");
            // Ici vous mettriez à jour votre UI de santé
        }

        private void HandleShieldChanged(float shield)
        {
            Debug.Log($"Bouclier: {shield}");
            // Ici vous mettriez à jour votre UI de bouclier
        }

        private void HandleStatsChanged()
        {
            Debug.Log("===== STATS DU JOUEUR =====");
            Debug.Log($"HP Max: {playerStats.MaxHp}");
            Debug.Log($"Dégâts: {playerStats.Damage}");
            Debug.Log($"Chance Critique: {playerStats.CriticalChance}%");
            Debug.Log($"Dégâts Critiques: {playerStats.CriticalDamage}%");
            Debug.Log($"Vitesse d'Attaque: {playerStats.AttackSpeed}/s");
            Debug.Log($"Nombre de Projectiles: {playerStats.ProjectileCount}");
            Debug.Log($"Portée: {playerStats.Range}");
            Debug.Log("============================");
            // Ici vous mettriez à jour votre UI de stats
        }

        private void HandlePlayerDied()
        {
            Debug.Log("Le joueur est mort!");
            // Ici vous afficheriez l'écran de game over
        }
        #endregion

        #region Handlers d'Events - Talents
        private void HandleTalentUnlocked(TalentNodeData talent, int points)
        {
            Debug.Log($"🌟 Talent débloqué: {talent.talentName} (Points: {points}/{talent.maxPoints})");
            // Ici vous afficheriez une notification ou animation
        }

        private void HandleTalentPointsChanged(int newPoints)
        {
            Debug.Log($"Points de talents disponibles: {newPoints}");
            // Ici vous mettriez à jour l'UI de points de talents
        }

        private void HandleLevelUp(int newLevel)
        {
            Debug.Log($"🎉 Level Up! Nouveau niveau: {newLevel}");
            // Ici vous afficheriez une animation de level up
        }

        private void HandleTalentTreeReset()
        {
            Debug.Log("Arbre de talents réinitialisé!");
            // Ici vous mettriez à jour l'UI de l'arbre de talents
        }
        #endregion

        #region Exemples d'utilisation
        // Ces méthodes montrent comment utiliser les différentes fonctionnalités

        /// <summary>
        /// Exemple: Simuler un combat
        /// </summary>
        [ContextMenu("Example: Simulate Combat")]
        private void ExampleCombat()
        {
            Debug.Log("=== SIMULATION DE COMBAT ===");
            
            // Le joueur prend des dégâts
            Debug.Log("Le joueur prend 30 points de dégâts...");
            playerStats.TakeDamage(30f);

            // Le joueur se soigne
            Debug.Log("Le joueur se soigne de 20 HP...");
            playerStats.Heal(20f);

            // Calcul de dégâts avec chance de critique
            for (int i = 0; i < 5; i++)
            {
                float damage = playerStats.CalculateDamage();
                Debug.Log($"Attaque #{i + 1}: {damage:F1} dégâts");
            }
        }

        /// <summary>
        /// Exemple: Débloquer un talent
        /// </summary>
        [ContextMenu("Example: Try Unlock First Talent")]
        private void ExampleUnlockTalent()
        {
            if (talentTreeManager.TalentTree == null)
            {
                Debug.LogError("Aucun TalentTree assigné!");
                return;
            }

            var rootNodes = talentTreeManager.TalentTree.GetRootNodes();
            
            if (rootNodes.Count > 0)
            {
                var firstNode = rootNodes[0];
                Debug.Log($"Tentative de déverrouillage: {firstNode.talentName}");
                
                bool success = talentTreeManager.TryUnlockTalent(firstNode.nodeId);
                
                if (success)
                {
                    Debug.Log($"✓ {firstNode.talentName} déverrouillé!");
                }
                else
                {
                    Debug.Log($"✗ Impossible de déverrouiller {firstNode.talentName}");
                }
            }
        }

        /// <summary>
        /// Exemple: Gagner un niveau
        /// </summary>
        [ContextMenu("Example: Gain Level")]
        private void ExampleGainLevel()
        {
            Debug.Log("Le joueur gagne un niveau!");
            talentTreeManager.LevelUp();
        }

        /// <summary>
        /// Exemple: Ajouter du bouclier
        /// </summary>
        [ContextMenu("Example: Add Shield")]
        private void ExampleAddShield()
        {
            Debug.Log("Ajout de 50 points de bouclier");
            playerStats.AddShield(50f);
        }

        /// <summary>
        /// Exemple: Sauvegarder et charger les talents
        /// </summary>
        [ContextMenu("Example: Save and Load Talents")]
        private void ExampleSaveLoad()
        {
            // Sauvegarder
            var saveData = talentTreeManager.GetSaveData();
            string json = JsonUtility.ToJson(saveData, true);
            Debug.Log("Données de sauvegarde:");
            Debug.Log(json);

            // Dans un vrai jeu, vous sauvegarderiez ceci dans un fichier ou PlayerPrefs
            // PlayerPrefs.SetString("TalentSave", json);

            // Charger (exemple)
            // string loadedJson = PlayerPrefs.GetString("TalentSave");
            // var loadedData = JsonUtility.FromJson<TalentTreeManager.TalentSaveData>(loadedJson);
            // talentTreeManager.LoadSaveData(loadedData);
        }
        #endregion

        private void OnDestroy()
        {
            // Nettoyer les écouteurs d'événements
            if (playerStats != null)
            {
                playerStats.OnHealthChanged -= HandleHealthChanged;
                playerStats.OnShieldChanged -= HandleShieldChanged;
                playerStats.OnStatsChanged -= HandleStatsChanged;
                playerStats.OnPlayerDied -= HandlePlayerDied;
            }

            if (talentTreeManager != null)
            {
                talentTreeManager.OnTalentUnlocked -= HandleTalentUnlocked;
                talentTreeManager.OnTalentPointsChanged -= HandleTalentPointsChanged;
                talentTreeManager.OnLevelUp -= HandleLevelUp;
                talentTreeManager.OnTalentTreeReset -= HandleTalentTreeReset;
            }
        }
    }
}
