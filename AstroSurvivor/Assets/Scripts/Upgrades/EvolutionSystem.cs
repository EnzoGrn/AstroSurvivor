using System.Collections.Generic;

namespace AstroSurvivor {

    public class EvolutionSystem {

        private List<UpgradeEvolutionData> _Evolutions;

        public EvolutionSystem(List<UpgradeEvolutionData> evolutions)
        {
            _Evolutions = evolutions;
        }

        public UpgradeData TryEvolve(PlayerBuildState state)
        {
            foreach (UpgradeEvolutionData evo in _Evolutions) {
                if (evo.CanEvolve(state))
                    return evo.ResultUpgrade;
            }

            return null;
        }
    }
}
