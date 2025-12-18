using System.Collections.Generic;

namespace AstroSurvivor {

    [System.Serializable]
    public class PlayerBuildState {

        public List<string> AcquiredUpgrades = new();

        public bool HasUpgrade(string id) => AcquiredUpgrades.Contains(id);

        public void AddUpgrade(string id)
        {
            if (!AcquiredUpgrades.Contains(id))
                AcquiredUpgrades.Add(id);
        }
    }
}
