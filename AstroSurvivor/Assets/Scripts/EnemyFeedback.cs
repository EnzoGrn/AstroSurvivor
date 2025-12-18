using UnityEngine;
using System.Collections;

public class EnemyFeedback : MonoBehaviour
{
    [Header("Silhouette Materials")]
    [SerializeField] private Material hitMaterial;
    [SerializeField] private Material deathMaterial;

    [Header("Durations")]
    [SerializeField] private float hitDuration = 0.08f;
    [SerializeField] private float deathDuration = 0.12f;

    private Renderer[] _renderers;
    private Material[][] _originalMaterials;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        CacheOriginalMaterials();
    }

    private void CacheOriginalMaterials()
    {
        _originalMaterials = new Material[_renderers.Length][];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].materials;
        }
    }

    public void OnHit()
    {
        StartOverride(hitMaterial, hitDuration);
    }

    public void OnDeath()
    {
        StartOverride(deathMaterial, deathDuration);
    }

    private void StartOverride(Material mat, float duration)
    {
        if (_flashCoroutine != null)
            StopCoroutine(_flashCoroutine);

        _flashCoroutine = StartCoroutine(OverrideRoutine(mat, duration));
    }

    private IEnumerator OverrideRoutine(Material mat, float duration)
    {
        SetMaterial(mat);
        yield return new WaitForSeconds(duration);
        RestoreMaterials();
    }

    private void SetMaterial(Material mat)
    {
        foreach (Renderer renderer in _renderers)
        {
            Material[] mats = new Material[renderer.materials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = mat;

            renderer.materials = mats;
        }
    }

    private void RestoreMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].materials = _originalMaterials[i];
        }
    }
}
