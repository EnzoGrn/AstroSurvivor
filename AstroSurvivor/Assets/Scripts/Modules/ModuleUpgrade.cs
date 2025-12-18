using UnityEngine;

namespace AstroSurvivor {

    [CreateAssetMenu(menuName = "Upgrades/Module Upgrade")]
    public class ModuleUpgradeData : UpgradeData {

        public ModuleBehaviour ModulePrefab;

        public override void Apply(PlayerContext context)
        {
            // context.Modules.SpawnModule(ModulePrefab);
        }
    }
}
