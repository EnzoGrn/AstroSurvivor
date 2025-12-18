using UnityEngine;

namespace AstroSurvivor {

    [CreateAssetMenu(menuName = "Upgrades/Mutation Upgrade")]
    public class MutationUpgradeData : UpgradeData {

        public override void Apply(PlayerContext context)
        {
            // context.weapons.EnableMutation(id);
        }
    }
}
