using System.Collections.Generic;
using UnityEngine;

namespace AstroSurvivor {

    public class PlayerModuleController {

        private List<ModuleBehaviour> _ActiveModules = new();

        private Transform _PlayerTransform;

        public PlayerModuleController(Transform player)
        {
            _PlayerTransform = player;
        }

        public void SpawnModule(ModuleBehaviour prefab)
        {
            var module = GameObject.Instantiate(prefab, _PlayerTransform.position, Quaternion.identity);

            module.Initialize(null);

            _ActiveModules.Add(module);
        }
    }
}
