using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace AstroSurvivor
{
    /// <summary>
    /// Composant UI pour un nœud de talent individuel
    /// </summary>
    public class TalentNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Références UI")]
        [SerializeField] private Image nodeIcon;
        [SerializeField] private Image nodeBackground;
        [SerializeField] private Image nodeBorder;
        [SerializeField] private TextMeshProUGUI pointsText;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private GameObject maxedOutIndicator;

        [Header("Couleurs")]
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color availableColor = new Color(1f, 1f, 0.5f, 1f);
        [SerializeField] private Color unlockedColor = new Color(0.5f, 1f, 0.5f, 1f);
        [SerializeField] private Color maxedOutColor = new Color(0.2f, 0.8f, 1f, 1f);
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 1f);

        [Header("Animation")]
        [SerializeField] private float hoverScale = 1.15f;
        [SerializeField] private float animationDuration = 0.2f;

        // Données
        private TalentNodeData nodeData;
        private TalentTreeManager treeManager;
        private TalentTreeUI treeUI;
        private int currentPoints;
        private bool isUnlocked;
        private bool canUnlock;
        private bool isMaxed;

        private Vector3 originalScale;
        private Color originalBorderColor;

        // État
        private bool isHovered;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        /// <summary>
        /// Initialise le nœud avec ses données
        /// </summary>
        public void Initialize(TalentNodeData data, TalentTreeManager manager, TalentTreeUI ui)
        {
            nodeData = data;
            treeManager = manager;
            treeUI = ui;

            // Configure l'icône
            if (nodeIcon != null && nodeData.icon != null)
            {
                nodeIcon.sprite = nodeData.icon;
            }

            // Configure la couleur de fond personnalisée
            if (nodeBackground != null)
            {
                nodeBackground.color = nodeData.nodeColor;
            }

            UpdateVisuals();
        }

        /// <summary>
        /// Met à jour l'apparence visuelle du nœud
        /// </summary>
        public void UpdateVisuals()
        {
            if (nodeData == null || treeManager == null) return;

            // Récupère l'état actuel
            isUnlocked = treeManager.IsTalentUnlocked(nodeData.nodeId);
            currentPoints = treeManager.GetTalentPoints(nodeData.nodeId);
            canUnlock = treeManager.CanUnlockTalent(nodeData);
            isMaxed = currentPoints >= nodeData.maxPoints;

            // Met à jour le texte des points
            if (pointsText != null)
            {
                if (isUnlocked && nodeData.maxPoints > 1)
                {
                    pointsText.text = $"{currentPoints}/{nodeData.maxPoints}";
                    pointsText.gameObject.SetActive(true);
                }
                else
                {
                    pointsText.gameObject.SetActive(false);
                }
            }

            // Met à jour les indicateurs visuels
            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!isUnlocked && !canUnlock);
            }

            if (maxedOutIndicator != null)
            {
                maxedOutIndicator.SetActive(isMaxed);
            }

            // Met à jour la couleur de la bordure
            Color targetColor;
            if (isMaxed)
            {
                targetColor = maxedOutColor;
            }
            else if (isUnlocked)
            {
                targetColor = unlockedColor;
            }
            else if (canUnlock)
            {
                targetColor = availableColor;
            }
            else
            {
                targetColor = lockedColor;
            }

            if (nodeBorder != null)
            {
                nodeBorder.color = targetColor;
                originalBorderColor = targetColor;
            }

            // Grise l'icône si verrouillé
            if (nodeIcon != null)
            {
                nodeIcon.color = isUnlocked || canUnlock ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;

            // Animation de survol
            StopAllCoroutines();
            StartCoroutine(ScaleAnimation(originalScale * hoverScale));

            // Change la couleur de la bordure
            if (nodeBorder != null)
            {
                nodeBorder.color = hoverColor;
            }

            // Affiche le tooltip
            if (treeUI != null && nodeData != null)
            {
                treeUI.ShowTooltip(nodeData, currentPoints, canUnlock, isMaxed);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;

            // Animation de sortie
            StopAllCoroutines();
            StartCoroutine(ScaleAnimation(originalScale));

            // Restaure la couleur de la bordure
            if (nodeBorder != null)
            {
                nodeBorder.color = originalBorderColor;
            }

            // Cache le tooltip
            if (treeUI != null)
            {
                treeUI.HideTooltip();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (nodeData == null || treeManager == null) return;

            // Tentative de déblocage/amélioration
            bool success = treeManager.TryUnlockTalent(nodeData.nodeId);

            if (success)
            {
                // Animation de succès
                PlayUnlockAnimation();

                // Met à jour l'UI
                UpdateVisuals();

                // Notifie l'arbre pour mettre à jour tous les nœuds
                if (treeUI != null)
                {
                    treeUI.RefreshAllNodes();
                }
            }
            else
            {
                // Animation d'échec (shake)
                PlayFailAnimation();
            }
        }

        private System.Collections.IEnumerator ScaleAnimation(Vector3 targetScale)
        {
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        private void PlayUnlockAnimation()
        {
            // Animation simple de "pop"
            StopAllCoroutines();
            StartCoroutine(UnlockAnimationCoroutine());
        }

        private System.Collections.IEnumerator UnlockAnimationCoroutine()
        {
            Vector3 targetScale = originalScale * 1.3f;

            // Grossit
            yield return ScaleAnimation(targetScale);

            // Revient à la normale (ou hover si toujours survolé)
            yield return ScaleAnimation(isHovered ? originalScale * hoverScale : originalScale);
        }

        private void PlayFailAnimation()
        {
            // Animation de "shake"
            StopAllCoroutines();
            StartCoroutine(ShakeAnimation());
        }

        private System.Collections.IEnumerator ShakeAnimation()
        {
            float duration = 0.3f;
            float magnitude = 10f;
            float elapsed = 0f;

            Vector3 originalPos = transform.localPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float x = Random.Range(-1f, 1f) * magnitude * (1 - elapsed / duration);
                float y = Random.Range(-1f, 1f) * magnitude * (1 - elapsed / duration);

                transform.localPosition = originalPos + new Vector3(x, y, 0);

                yield return null;
            }

            transform.localPosition = originalPos;
        }

        /// <summary>
        /// Retourne les données du nœud
        /// </summary>
        public TalentNodeData GetNodeData() => nodeData;

        /// <summary>
        /// Retourne si le nœud est débloqué
        /// </summary>
        public bool IsUnlocked() => isUnlocked;
    }
}
