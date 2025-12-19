using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstroSurvivor
{
    /// <summary>
    /// Affiche un tooltip avec les informations détaillées d'un talent
    /// </summary>
    public class TalentTooltipUI : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI requirementsText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Configuration")]
        [SerializeField] private float fadeDuration = 0.15f;
        [SerializeField] private Vector2 offset = new Vector2(20, 20);

        private RectTransform rectTransform;
        private Canvas parentCanvas;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();

            // Cache le tooltip par défaut
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Affiche le tooltip avec les informations du talent
        /// </summary>
        public void Show(TalentNodeData nodeData, int currentPoints, bool canUnlock, bool isMaxed)
        {
            if (nodeData == null) return;

            // Titre
            if (titleText != null)
            {
                titleText.text = nodeData.talentName;
            }

            // Description complète (inclut les modificateurs)
            if (descriptionText != null)
            {
                descriptionText.text = nodeData.GetFullDescription();
            }

            // Coût
            if (costText != null)
            {
                if (isMaxed)
                {
                    costText.text = "<color=cyan>Points maximums atteints!</color>";
                }
                else
                {
                    costText.text = $"Coût: {nodeData.pointCost} point(s) de talent";
                }
            }

            // Prérequis
            if (requirementsText != null)
            {
                string reqText = "";

                if (nodeData.requiredLevel > 1)
                {
                    reqText += $"Niveau d'arbre requis: {nodeData.requiredLevel}\n";
                }

                if (nodeData.parentNodeIds.Count > 0)
                {
                    reqText += "Requiert un des talents parents";
                }

                requirementsText.text = reqText;
                requirementsText.gameObject.SetActive(!string.IsNullOrEmpty(reqText));
            }

            // Statut actuel
            if (statusText != null)
            {
                string status = "";

                if (isMaxed)
                {
                    status = "<color=cyan>★ MAXIMISÉ ★</color>";
                }
                else if (currentPoints > 0)
                {
                    status = $"<color=lime>✓ Débloqué ({currentPoints}/{nodeData.maxPoints})</color>";
                }
                else if (canUnlock)
                {
                    status = "<color=yellow>! Disponible - Cliquez pour débloquer</color>";
                }
                else
                {
                    status = "<color=red>✗ Verrouillé</color>";
                }

                statusText.text = status;
            }

            // Affiche le tooltip
            ShowTooltip();
        }

        /// <summary>
        /// Cache le tooltip
        /// </summary>
        public void Hide()
        {
            HideTooltip();
        }

        /// <summary>
        /// Met à jour la position du tooltip pour suivre le curseur
        /// </summary>
        public void UpdatePosition(Vector2 mousePosition)
        {
            if (rectTransform == null || parentCanvas == null) return;

            // Convertit la position de la souris en position locale du canvas
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePosition,
                parentCanvas.worldCamera,
                out localPoint
            );

            // Applique l'offset
            rectTransform.anchoredPosition = localPoint + offset;

            // Empêche le tooltip de sortir de l'écran
            ClampToScreen();
        }

        private void ClampToScreen()
        {
            if (rectTransform == null || parentCanvas == null) return;

            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            RectTransform canvasRect = parentCanvas.transform as RectTransform;
            Vector3[] canvasCorners = new Vector3[4];
            canvasRect.GetWorldCorners(canvasCorners);

            Vector2 anchoredPos = rectTransform.anchoredPosition;

            // Vérifie les bords
            if (corners[2].x > canvasCorners[2].x) // Droit
            {
                anchoredPos.x -= corners[2].x - canvasCorners[2].x;
            }
            if (corners[0].x < canvasCorners[0].x) // Gauche
            {
                anchoredPos.x += canvasCorners[0].x - corners[0].x;
            }
            if (corners[2].y > canvasCorners[2].y) // Haut
            {
                anchoredPos.y -= corners[2].y - canvasCorners[2].y;
            }
            if (corners[0].y < canvasCorners[0].y) // Bas
            {
                anchoredPos.y += canvasCorners[0].y - corners[0].y;
            }

            rectTransform.anchoredPosition = anchoredPos;
        }

        private void ShowTooltip()
        {
            if (canvasGroup != null)
            {
                StopAllCoroutines();
                StartCoroutine(FadeIn());
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void HideTooltip()
        {
            if (canvasGroup != null)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOut());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator FadeIn()
        {
            canvasGroup.blocksRaycasts = false;
            float elapsed = 0;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;

                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);

                yield return null;
            }

            canvasGroup.alpha = 1;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            canvasGroup.blocksRaycasts = false;
            float elapsed = 0;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsed / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0;
        }

        private void Update()
        {
            // Met à jour la position si visible
            if (canvasGroup != null && canvasGroup.alpha > 0)
            {
                UpdatePosition(Input.mousePosition);
            }
        }
    }
}
