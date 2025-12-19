using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstroSurvivor
{
    /// <summary>
    /// Gère l'affichage complet de l'arbre de talents
    /// </summary>
    public class TalentTreeUI : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private TalentTree talentTree;
        [SerializeField] private TalentTreeManager treeManager;
        [SerializeField] private PlayerStats playerStats;

        [Header("Préfabs")]
        [SerializeField] private GameObject nodeUIPrefab;
        [SerializeField] private GameObject connectionLinePrefab;

        [Header("Containers")]
        [SerializeField] private RectTransform nodesContainer;
        [SerializeField] private RectTransform linesContainer;

        [Header("UI Info")]
        [SerializeField] private TextMeshProUGUI availablePointsText;
        [SerializeField] private TextMeshProUGUI treeLevelText;
        [SerializeField] private TextMeshProUGUI treeNameText;
        [SerializeField] private Button resetButton;

        [Header("Tooltip")]
        [SerializeField] private TalentTooltipUI tooltip;

        [Header("Configuration")]
        [SerializeField] private Color connectionLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private Color activeConnectionColor = new Color(0.5f, 1f, 0.5f, 1f);
        [SerializeField] private float connectionLineWidth = 3f;

        // Données internes
        private Dictionary<string, TalentNodeUI> nodeUIElements = new Dictionary<string, TalentNodeUI>();
        private List<Image> connectionLines = new List<Image>();

        private void Start()
        {
            if (treeManager == null)
            {
                treeManager = FindFirstObjectByType<TalentTreeManager>();
            }

            if (playerStats == null)
            {
                playerStats = FindFirstObjectByType<PlayerStats>();
            }

            // Configure le bouton de reset
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetButtonClicked);
            }

            // Initialise l'UI
            InitializeTreeUI();
        }

        /// <summary>
        /// Initialise tout l'arbre de talents
        /// </summary>
        private void InitializeTreeUI()
        {
            if (talentTree == null)
            {
                Debug.LogError("TalentTree non assigné dans TalentTreeUI!");
                return;
            }

            if (treeManager == null)
            {
                Debug.LogError("TalentTreeManager non trouvé!");
                return;
            }

            // Affiche les infos de l'arbre
            if (treeNameText != null)
            {
                treeNameText.text = talentTree.treeName;
            }

            // Crée tous les nœuds
            CreateAllNodes();

            // Crée les lignes de connexion
            CreateConnectionLines();

            // Met à jour l'affichage
            RefreshAllNodes();
            UpdateInfoPanel();
        }

        /// <summary>
        /// Crée tous les nœuds visuels
        /// </summary>
        private void CreateAllNodes()
        {
            if (nodeUIPrefab == null || nodesContainer == null)
            {
                Debug.LogError("NodeUIPrefab ou NodesContainer non assigné!");
                return;
            }

            // Nettoie les nœuds existants
            foreach (Transform child in nodesContainer)
            {
                Destroy(child.gameObject);
            }
            nodeUIElements.Clear();

            // Crée un nœud pour chaque TalentNodeData
            foreach (var nodeData in talentTree.talentNodes)
            {
                GameObject nodeObj = Instantiate(nodeUIPrefab, nodesContainer);
                TalentNodeUI nodeUI = nodeObj.GetComponent<TalentNodeUI>();

                if (nodeUI != null)
                {
                    // Initialise le nœud
                    nodeUI.Initialize(nodeData, treeManager, this);

                    // Positionne le nœud selon treePosition
                    RectTransform rect = nodeObj.GetComponent<RectTransform>();
                    if (rect != null)
                    {
                        rect.anchoredPosition = nodeData.treePosition;
                    }

                    // Stocke la référence
                    nodeUIElements[nodeData.nodeId] = nodeUI;
                }
            }
        }

        /// <summary>
        /// Crée les lignes de connexion entre les nœuds
        /// </summary>
        private void CreateConnectionLines()
        {
            if (linesContainer == null) return;

            // Nettoie les lignes existantes
            foreach (Transform child in linesContainer)
            {
                Destroy(child.gameObject);
            }
            connectionLines.Clear();

            // Crée une ligne pour chaque connexion parent-enfant
            foreach (var nodeData in talentTree.talentNodes)
            {
                if (!nodeUIElements.ContainsKey(nodeData.nodeId)) continue;

                TalentNodeUI childNode = nodeUIElements[nodeData.nodeId];

                foreach (string parentId in nodeData.parentNodeIds)
                {
                    if (!nodeUIElements.ContainsKey(parentId)) continue;

                    TalentNodeUI parentNode = nodeUIElements[parentId];

                    // Crée une ligne entre parent et enfant
                    CreateConnectionLine(parentNode.transform as RectTransform, childNode.transform as RectTransform);
                }
            }

            UpdateConnectionLines();
        }

        /// <summary>
        /// Crée une ligne de connexion entre deux nœuds
        /// </summary>
        private void CreateConnectionLine(RectTransform from, RectTransform to)
        {
            GameObject lineObj = new GameObject("ConnectionLine", typeof(RectTransform), typeof(Image));
            lineObj.transform.SetParent(linesContainer, false);

            Image lineImage = lineObj.GetComponent<Image>();
            lineImage.color = connectionLineColor;

            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            
            // Positionne et oriente la ligne
            Vector2 fromPos = from.anchoredPosition;
            Vector2 toPos = to.anchoredPosition;
            Vector2 direction = toPos - fromPos;
            float distance = direction.magnitude;

            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.sizeDelta = new Vector2(distance, connectionLineWidth);
            lineRect.anchoredPosition = fromPos + direction * 0.5f;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);

            // Met la ligne derrière les nœuds
            lineObj.transform.SetAsFirstSibling();

            connectionLines.Add(lineImage);
        }

        /// <summary>
        /// Met à jour les couleurs des lignes de connexion
        /// </summary>
        private void UpdateConnectionLines()
        {
            int lineIndex = 0;

            foreach (var nodeData in talentTree.talentNodes)
            {
                foreach (string parentId in nodeData.parentNodeIds)
                {
                    if (lineIndex >= connectionLines.Count) break;

                    Image lineImage = connectionLines[lineIndex];
                    
                    // La ligne est active si le nœud enfant est débloqué
                    bool isActive = treeManager.IsTalentUnlocked(nodeData.nodeId);
                    lineImage.color = isActive ? activeConnectionColor : connectionLineColor;

                    lineIndex++;
                }
            }
        }

        /// <summary>
        /// Rafraîchit tous les nœuds de l'arbre
        /// </summary>
        public void RefreshAllNodes()
        {
            foreach (var nodeUI in nodeUIElements.Values)
            {
                nodeUI.UpdateVisuals();
            }

            UpdateConnectionLines();
            UpdateInfoPanel();
        }

        /// <summary>
        /// Met à jour le panneau d'informations (points disponibles, niveau, etc.)
        /// </summary>
        private void UpdateInfoPanel()
        {
            if (treeManager == null) return;

            if (availablePointsText != null)
            {
                int availablePoints = treeManager.AvailableTalentPoints;
                availablePointsText.text = $"Points disponibles: {availablePoints}";

                // Change la couleur si aucun point disponible
                availablePointsText.color = availablePoints > 0 ? Color.yellow : Color.gray;
            }

            if (treeLevelText != null)
            {
                treeLevelText.text = $"Niveau d'arbre: {treeManager.treeLevel}";
            }
        }

        /// <summary>
        /// Affiche le tooltip pour un talent
        /// </summary>
        public void ShowTooltip(TalentNodeData nodeData, int currentPoints, bool canUnlock, bool isMaxed)
        {
            if (tooltip != null)
            {
                tooltip.Show(nodeData, currentPoints, canUnlock, isMaxed);
            }
        }

        /// <summary>
        /// Cache le tooltip
        /// </summary>
        public void HideTooltip()
        {
            if (tooltip != null)
            {
                tooltip.Hide();
            }
        }

        /// <summary>
        /// Appelé quand le bouton Reset est cliqué
        /// </summary>
        private void OnResetButtonClicked()
        {
            if (treeManager != null)
            {
                // Vous pouvez ajouter une confirmation ici
                treeManager.ResetTalents();
                RefreshAllNodes();
            }
        }

        /// <summary>
        /// Ouvre l'UI de l'arbre de talents
        /// </summary>
        public void OpenTalentTree()
        {
            gameObject.SetActive(true);
            RefreshAllNodes();
        }

        /// <summary>
        /// Ferme l'UI de l'arbre de talents
        /// </summary>
        public void CloseTalentTree()
        {
            HideTooltip();
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Rafraîchit quand l'UI est activée
            RefreshAllNodes();
        }

        /// <summary>
        /// Abonne aux événements du TalentTreeManager
        /// </summary>
        private void SubscribeToEvents()
        {
            if (treeManager != null)
            {
                treeManager.OnTalentUnlocked += OnTalentUnlockedEvent;
                treeManager.OnTalentPointsChanged += OnTalentPointsChangedEvent;
                treeManager.OnTalentsReset += OnTalentsResetEvent;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (treeManager != null)
            {
                treeManager.OnTalentUnlocked -= OnTalentUnlockedEvent;
                treeManager.OnTalentPointsChanged -= OnTalentPointsChangedEvent;
                treeManager.OnTalentsReset -= OnTalentsResetEvent;
            }
        }

        private void OnTalentUnlockedEvent(string nodeId, int points)
        {
            RefreshAllNodes();
        }

        private void OnTalentPointsChangedEvent(int newPoints)
        {
            UpdateInfoPanel();
        }

        private void OnTalentsResetEvent()
        {
            RefreshAllNodes();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(OnResetButtonClicked);
            }
        }
    }
}
