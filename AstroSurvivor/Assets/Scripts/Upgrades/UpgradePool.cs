using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AstroSurvivor {

    [CreateAssetMenu(menuName = "Upgrades/Upgrade Pool")]
    public class UpgradePool : ScriptableObject {

        public List<UpgradeData> Upgrades;

        public List<UpgradeData> GetRandomChoices(int count)
        {
            return Upgrades.OrderBy(x => Random.value).Take(count).ToList();
        }
    }
}
