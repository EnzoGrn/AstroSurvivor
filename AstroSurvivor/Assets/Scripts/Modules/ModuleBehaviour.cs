using UnityEngine;

namespace AstroSurvivor {

    public abstract class ModuleBehaviour : MonoBehaviour {

        protected PlayerContext Context;

        public virtual void Initialize(PlayerContext ctx)
        {
            Context = ctx;
        }
    }
}
