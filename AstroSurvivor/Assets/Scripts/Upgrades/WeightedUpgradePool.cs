using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor {

    [System.Serializable]
    public class RarityWeight {

        public UpgradeRarity Rarity;

        public float Weight;
    }

    [CreateAssetMenu(menuName = "Upgrades/Weighted Upgrade Pool")]
    public class WeightedUpgradePool : ScriptableObject {

        public List<UpgradeData> Upgrades;
        public List<RarityWeight> RarityWeights;

        public List<UpgradeData> GetRandomChoices(int count)
        {
            List<UpgradeData> results   = new();
            List<UpgradeData> available = new(Upgrades);

            for (int i = 0; i < count && available.Count > 0; i++) {
                UpgradeData selected = Roll(available);

                results.Add(selected);
                available.Remove(selected);
            }

            return results;
        }

        private UpgradeData Roll(List<UpgradeData> pool)
        {
            float totalWeight = 0f;

            foreach (var u in pool)
                totalWeight += GetWeight(u.Rarity);
            float roll = UnityEngine.Random.value * totalWeight;

            foreach (var u in pool) {
                roll -= GetWeight(u.Rarity);

                if (roll <= 0f)
                    return u;
            }

            return pool[0];
        }

        private float GetWeight(UpgradeRarity rarity)
        {
            return RarityWeights.Find(r => r.Rarity == rarity)?.Weight ?? 1f;
        }
    }
}
