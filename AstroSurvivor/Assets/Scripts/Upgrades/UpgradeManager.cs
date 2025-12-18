using UnityEngine;
using System.Collections.Generic;

namespace AstroSurvivor {

    public class UpgradeManager : MonoBehaviour {

        public UpgradePool pool;

        public PlayerContext context;

        public void OfferUpgrades()
        {
            List<UpgradeData> choices = pool.GetRandomChoices(3);

            // TODO: Send to UI
        }

        public void SelectUpgrade(UpgradeData upgrade)
        {
            upgrade.Apply(context);
        }
    }
}
