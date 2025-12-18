using System.Collections;
using UnityEngine;

namespace AstroSurvivor.UI {

    public class UpgradePanelUI : MonoBehaviour {

        public CanvasGroup canvasGroup;

        public float fadeDuration = 0.25f;

        void Awake()
        {
            SetVisible(false, true);
        }

        public void SetVisible(bool visible, bool instant = false)
        {
            gameObject.SetActive(true);

            StopAllCoroutines();

            if (instant) {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
                canvasGroup.blocksRaycasts = visible;

                if (!visible)
                    gameObject.SetActive(false);
                return;
            }

            StartCoroutine(FadeRoutine(visible));
        }

        private IEnumerator FadeRoutine(bool visible)
        {
            float start = canvasGroup.alpha;
            float target = visible ? 1f : 0f;
            float t = 0f;

            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;

            while (t < fadeDuration) {
                t += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);

                yield return null;
            }

            canvasGroup.alpha = target;

            if (!visible)
                gameObject.SetActive(false);
        }
    }
}
