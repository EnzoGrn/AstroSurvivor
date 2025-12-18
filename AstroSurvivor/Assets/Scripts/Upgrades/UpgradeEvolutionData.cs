using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor {

    [CreateAssetMenu(menuName = "Upgrades/Upgrade Evolution")]
    public class UpgradeEvolutionData : ScriptableObject {

        public List<string> RequiredUpgradeIds;

        public UpgradeData ResultUpgrade;

        public bool CanEvolve(PlayerBuildState state)
        {
            foreach (var id in RequiredUpgradeIds) {
                if (!state.HasUpgrade(id))
                    return false;
            }

            return true;
        }
    }
}
