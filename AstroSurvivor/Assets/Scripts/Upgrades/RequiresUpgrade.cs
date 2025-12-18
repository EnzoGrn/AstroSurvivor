using System.Collections.Generic;

namespace AstroSurvivor {

    public interface IPrerequisite {

        bool IsMet(PlayerBuildState state);
    }

    [System.Serializable]
    public class RequiresUpgrade : IPrerequisite {

        public string RequiredUpgradeId;

        public bool IsMet(PlayerBuildState state)
        {
            return state.HasUpgrade(RequiredUpgradeId);
        }
    }
}
