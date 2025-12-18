using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor {

    public abstract class UpgradeData : ScriptableObject {

        [Header("Identity")]
        public string ID;
        public string DisplayName;

        [TextArea]
        public string Description;

        [Header("Meta")]
        public UpgradeType Type;
        public UpgradeRarity Rarity;

        public Sprite Icon;

        [Header("Prerequisites")]
        public List<RequiresUpgrade> Prerequisites = new();

        // Called once when the upgrade is picked
        public abstract void Apply(PlayerContext context);

        public bool CanBeOffered(PlayerBuildState state)
        {
            foreach (var p in Prerequisites) {
                if (!p.IsMet(state))
                    return false;
            }

            return true;
        }
    }
}
