using System.Collections;
using TMPro;
using UnityEngine;

namespace AstroSurvivor.UI {

    public class WaveUI : MonoBehaviour {

        [Header("UI")]
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI waveText;

        [Header("Timing")]
        public float fadeDuration = 0.5f;
        public float displayDuration = 1.5f;

        private Coroutine currentRoutine;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        public void ShowWave(int waveNumber)
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);
            waveText.text = $"WAVE {waveNumber}\n<size=60%>Survive</size>";
            currentRoutine = StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            yield return Fade(0f, 1f);

            yield return new WaitForSeconds(displayDuration);

            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            float t = 0f;

            while (t < fadeDuration) {
                t += Time.deltaTime;

                canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);

                yield return null;
            }

            canvasGroup.alpha = to;
        }
    }
}
