using UnityEngine;

namespace AstroSurvivor.Modules {

    public class ShieldDrone : ModuleBehaviour {

        [Header("Orbit")]
        public Transform Pivot; // normalement le joueur ou ModuleAncho

        public float OrbitRadius = 2f;
        public float OrbitSpeed = 90f; // degrés/sec
        private float CurrentAngle = 0f;

        [Header("Shield")]
        public LayerMask ProjectileLayer;

        public ParticleSystem ImpactEffect;

        [Header("Health / Regen")]
        public int MaxBlocks = 3;
        public float RegenDelay = 5f;
        public float RegenSpeed = 1f;

        private int _CurrentBlocks;

        private float _LastHitTime;

        private void Awake()
        {
            _CurrentBlocks = MaxBlocks;
        }

        private void Update()
        {
            OrbitAroundPivot();
            HandleRegen();
        }

        public override void Initialize(PlayerContext ctx)
        {
            base.Initialize(ctx);

            Pivot = ctx.Stats.transform;

            transform.rotation = ctx.Stats.transform.rotation;
        }

        private void OrbitAroundPivot()
        {
            CurrentAngle += OrbitSpeed * Time.deltaTime;
            CurrentAngle %= 360f;

            float rad = CurrentAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * OrbitRadius;

            transform.position = Pivot.position + offset;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (((1 << other.gameObject.layer) & ProjectileLayer) != 0)
                BlockProjectile(other.gameObject);
        }

        private void BlockProjectile(GameObject projectile)
        {
            Destroy(projectile);

            if (ImpactEffect != null) {
                ImpactEffect.transform.position = projectile.transform.position;

                ImpactEffect.Play();
            }

            Vector3 dir = (projectile.transform.position - Pivot.position).normalized;

            transform.up = dir;

            _CurrentBlocks--;

            _LastHitTime = Time.time;

            if (_CurrentBlocks <= 0)
                DestroyDrone();
        }

        private void HandleRegen()
        {
            if (_CurrentBlocks < MaxBlocks && Time.time - _LastHitTime >= RegenDelay) {
                _CurrentBlocks += Mathf.CeilToInt(RegenSpeed * Time.deltaTime);

                if (_CurrentBlocks > MaxBlocks)
                    _CurrentBlocks = MaxBlocks;
            }
        }

        private void DestroyDrone()
        {
            // Animation ou effet ici
            Destroy(gameObject);
        }
    }

}
