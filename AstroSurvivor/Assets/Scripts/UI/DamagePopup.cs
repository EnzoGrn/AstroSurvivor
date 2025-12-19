using TMPro;
using UnityEngine;

namespace AstroSurvivor {

    public class DamagePopup : MonoBehaviour {

        [SerializeField] private TMP_Text text;
        [SerializeField] private float floatSpeed = 40f;
        [SerializeField] private float lifetime = 0.5f;

        private RectTransform rect;
        private CanvasGroup canvasGroup;
        private float timer;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Init(int damage, bool critic, Vector3 worldPosition, Camera cam)
        {
            text.text = damage.ToString();

            if (critic) {
                text.color = Color.yellow;
                rect.localScale = Vector3.one * 1.3f;
            } else {
                text.color = Color.white;
                rect.localScale = Vector3.one;
            }

            Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
 
            rect.position = screenPos;

            timer = lifetime;
        }

        private void Update()
        {
            rect.position += Vector3.up * floatSpeed * Time.deltaTime;

            timer -= Time.deltaTime;
            canvasGroup.alpha = timer / lifetime;

            if (timer <= 0f)
                Destroy(gameObject);
        }
    }
}
