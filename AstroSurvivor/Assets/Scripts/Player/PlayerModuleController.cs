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

        public void SpawnModule(PlayerContext context, ModuleBehaviour prefab)
        {
            var module = GameObject.Instantiate(prefab, _PlayerTransform.position, Quaternion.identity);

            module.transform.parent = _PlayerTransform;

            module.Initialize(context);

            _ActiveModules.Add(module);
        }
    }
}
