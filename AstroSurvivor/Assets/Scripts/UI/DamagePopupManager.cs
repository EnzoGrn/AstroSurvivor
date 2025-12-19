using UnityEngine;

namespace AstroSurvivor {

    public class DamagePopupManager : MonoBehaviour {

        public static DamagePopupManager Instance;

        [SerializeField] private DamagePopup popupPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowDamage(int damage, Vector3 worldPosition, bool critic = false)
        {
            DamagePopup popup = Instantiate(popupPrefab, canvas.transform);

            popup.Init(damage, critic, worldPosition, mainCamera);
        }
    }
}
