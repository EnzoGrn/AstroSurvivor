using UnityEngine;
using UnityEngine.InputSystem;

namespace AstroSurvivor
{
    /// <summary>
    /// Gère l'ouverture et la fermeture de l'UI de l'arbre de talents
    /// </summary>
    public class TalentTreeUIController : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private TalentTreeUI talentTreeUI;
        [SerializeField] private GameObject talentTreePanel;

        [Header("Configuration")]
        [SerializeField] private KeyCode openKey = KeyCode.T;
        [SerializeField] private bool pauseGameWhenOpen = true;

        private bool isOpen = false;

        private void Start()
        {
            // Cache l'UI au démarrage
            if (talentTreePanel != null)
            {
                talentTreePanel.SetActive(false);
            }
            else if (talentTreeUI != null)
            {
                talentTreeUI.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Écoute l'appui sur la touche
            if (Input.GetKeyDown(openKey))
            {
                ToggleTalentTree();
            }

            // Alternative : touche Échap pour fermer
            if (isOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseTalentTree();
            }
        }

        /// <summary>
        /// Bascule l'affichage de l'arbre de talents
        /// </summary>
        public void ToggleTalentTree()
        {
            if (isOpen)
            {
                CloseTalentTree();
            }
            else
            {
                OpenTalentTree();
            }
        }

        /// <summary>
        /// Ouvre l'UI de l'arbre de talents
        /// </summary>
        public void OpenTalentTree()
        {
            isOpen = true;

            // Active l'UI
            if (talentTreePanel != null)
            {
                talentTreePanel.SetActive(true);
            }
            else if (talentTreeUI != null)
            {
                talentTreeUI.gameObject.SetActive(true);
            }

            // Appelle la méthode d'ouverture si elle existe
            if (talentTreeUI != null)
            {
                talentTreeUI.OpenTalentTree();
            }

            // Met le jeu en pause si configuré
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 0f;
            }

            // Change le curseur
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            Debug.Log("Arbre de talents ouvert (Touche: " + openKey + ")");
        }

        /// <summary>
        /// Ferme l'UI de l'arbre de talents
        /// </summary>
        public void CloseTalentTree()
        {
            isOpen = false;

            // Appelle la méthode de fermeture si elle existe
            if (talentTreeUI != null)
            {
                talentTreeUI.CloseTalentTree();
            }

            // Désactive l'UI
            if (talentTreePanel != null)
            {
                talentTreePanel.SetActive(false);
            }
            else if (talentTreeUI != null)
            {
                talentTreeUI.gameObject.SetActive(false);
            }

            // Reprend le jeu si il était en pause
            if (pauseGameWhenOpen)
            {
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// Retourne si l'arbre de talents est actuellement ouvert
        /// </summary>
        public bool IsOpen() => isOpen;

        private void OnDestroy()
        {
            // S'assure que le temps reprend normalement si l'objet est détruit
            if (isOpen && pauseGameWhenOpen)
            {
                Time.timeScale = 1f;
            }
        }
    }
}
