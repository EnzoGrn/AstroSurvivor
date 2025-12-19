using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AstroSurvivor.UI {

    using AstroSurvivor;
    using System;

    public class UpgradeUIController : MonoBehaviour {

        [Header("Data")]
        public WeightedUpgradePool UpgradePool;

        [Header("UI")]
        public UpgradePanelUI Panel;
        public UpgradeCardUI[] Cards; // 3 cards

        private PlayerContext _Context;
        private PlayerBuildState _BuildState;

        public Action OnSelected;

        public void Initialize(PlayerContext ctx, PlayerBuildState state)
        {
            _Context = ctx;
            _BuildState = state;
        }

        public bool Open()
        {
            List<UpgradeData> upgrades = RollValidUpgrades(Cards.Length);

            if (upgrades == null || upgrades.Count == 0)
                return false;
            Panel.SetVisible(true);

            for (int i = 0; i < Cards.Length; i++) {
                if (i < upgrades.Count) {
                    Cards[i].gameObject.SetActive(true);

                    Cards[i].Bind(upgrades[i], OnUpgradeSelected);
                } else {
                    Cards[i].gameObject.SetActive(false);
                }
            }

            return true;
        }

        public void Close()
        {
            Panel.SetVisible(false);
        }

        private List<UpgradeData> RollValidUpgrades(int count)
        {
            var pool = UpgradePool.Upgrades.Where(u => !_BuildState.HasUpgrade(u.ID)).Where(u => u.CanBeOffered(_BuildState)).ToList();

            return RollWeighted(pool, count);
        }

        private List<UpgradeData> RollWeighted(List<UpgradeData> pool, int count)
        {
            List<UpgradeData> results = new();

            for (int i = 0; i < count && pool.Count > 0; i++) {
                UpgradeData selected = UpgradePool.Roll(pool);

                results.Add(selected);
                pool.Remove(selected);
            }

            return results;
        }

        private void OnUpgradeSelected(UpgradeData upgrade)
        {
            upgrade.Apply(_Context);

            _BuildState.AddUpgrade(upgrade.ID);
            
            BuildSaveSystem.Save(_BuildState);

            Close();

            OnSelected?.Invoke();
        }
    }
}
