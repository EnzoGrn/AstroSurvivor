using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AstroSurvivor {

    public class HealthBarUI : MonoBehaviour {

        private PlayerStats healthSystem;

        public Slider slider;
        public TMP_Text text;

        public void Awake()
        {
            healthSystem = FindFirstObjectByType<PlayerStats>();

            healthSystem.OnHealthChanged += UpdateUIHealth;

            UpdateUIHealth(healthSystem.CurrentHp, healthSystem.MaxHp);
        }

        private void UpdateUIHealth(float currentHealth, float maxHealth)
        {
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = (float)currentHealth / maxHealth;

            text.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }

        private void OnDestroy()
        {
            if (healthSystem != null)
                healthSystem.OnHealthChanged -= UpdateUIHealth;
        }
    }
}
