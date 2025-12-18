using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AstroSurvivor.UI {

    [RequireComponent(typeof(Button))]
    public class UpgradeCardUI : MonoBehaviour {

        [Header("UI")]
        public Image Background;
        public Image Icon;

        public TMP_Text Title;
        public TMP_Text Description;
        public TMP_Text Rarity;

        [Header("Rarity Colors")]
        public Color CommonColor;
        public Color RareColor;
        public Color EpicColor;
        public Color LegendaryColor;

        private UpgradeData Data;

        public void Bind(UpgradeData upgrade, System.Action<UpgradeData> onClick)
        {
            Data = upgrade;

            if (Icon && upgrade.Icon != null)
                Icon.sprite = upgrade.Icon;
            Title.text = upgrade.DisplayName;
            Description.text = upgrade.Description;
            Rarity.text = upgrade.Rarity.ToString();

            Color color = GetRarityColor(upgrade.Rarity);

            Background.color = color;
            Rarity.color = color;

            var button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(Data));
        }

        private Color GetRarityColor(UpgradeRarity rarity)
        {
            return rarity switch {
                UpgradeRarity.Common => CommonColor,
                UpgradeRarity.Rare => RareColor,
                UpgradeRarity.Epic => EpicColor,
                UpgradeRarity.Legendary => LegendaryColor,
                _ => Color.white
            };
        }
    }
}
