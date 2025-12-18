using System.Collections.Generic;

namespace AstroSurvivor {

    public class PlayerWeaponController {

        private HashSet<string> _ActiveMutations = new();

        public void EnableMutation(string mutationId)
        {
            _ActiveMutations.Add(mutationId);
        }

        public bool HasMutation(string id) => _ActiveMutations.Contains(id);
    }
}
