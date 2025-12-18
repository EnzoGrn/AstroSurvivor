using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private int _damage;
    private float _lifetime = 3f;
    private float _timer;

    [Header("Trail Settings")]
    [SerializeField] private float trailTime = 0.3f;
    [SerializeField] private float trailStartWidth = 0.15f;
    [SerializeField] private float trailEndWidth = 0.05f;
    [SerializeField] private Color trailStartColor = new Color(1f, 0.8f, 0f, 1f); // Orange/jaune
    [SerializeField] private Color trailEndColor = new Color(1f, 0.3f, 0f, 0f); // Rouge transparent
    [SerializeField] private Material trailMaterial;

    private TrailRenderer trail;

    private void Awake()
    {
        SetupTrail();
    }

    public void Fire(Vector3 direction, float speed, int damage)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _timer = 0f;

        // Réinitialise le trail quand on tire
        if (trail != null)
        {
            trail.Clear();
        }
    }

    private void Update()
    {
        transform.position += _direction * _speed * Time.deltaTime;
        _timer += Time.deltaTime;
        if (_timer >= _lifetime)
        {
            Disable();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile collided with " + other.name);
        if (other.GetComponent<Enemy>() is Enemy enemy)
        {
            enemy.TakeDamage(_damage);
            Disable();
        }
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void SetupTrail()
    {
        trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }

        trail.time = trailTime;
        trail.startWidth = trailStartWidth;
        trail.endWidth = trailEndWidth;
        trail.minVertexDistance = 0.05f;
        trail.numCornerVertices = 5;
        trail.numCapVertices = 5;
        trail.alignment = LineAlignment.View;
        trail.textureMode = LineTextureMode.Stretch;

        // Configuration du gradient avec dégradé de couleur
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(trailStartColor, 0f),
                new GradientColorKey(trailEndColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(trailStartColor.a, 0f),
                new GradientAlphaKey(trailEndColor.a, 1f)
            }
        );
        trail.colorGradient = gradient;

        // Material avec shader Additive pour un effet lumineux
        if (trailMaterial != null)
        {
            trail.material = trailMaterial;
        }
        else
        {
            Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
            mat.SetColor("_Color", trailStartColor);
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One); // Additive
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            trail.material = mat;
        }

        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
    }

    // Méthode utile pour changer la couleur du trail dynamiquement
    public void SetTrailColor(Color startColor, Color endColor)
    {
        trailStartColor = startColor;
        trailEndColor = endColor;
        if (trail != null)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(endColor, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(startColor.a, 0f),
                    new GradientAlphaKey(endColor.a, 1f)
                }
            );
            trail.colorGradient = gradient;
        }
    }

    public void SetTrailColor(Color color)
    {
        SetTrailColor(color, new Color(color.r, color.g, color.b, 0f));
    }
}