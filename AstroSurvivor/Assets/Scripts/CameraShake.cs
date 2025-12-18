using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _originalPosition = transform.position;
    }

    public void Shake(float duration = 0.1f, float strength = 0.2f)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float timer = 0f;

        while (timer < duration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * strength;
            randomOffset.y = 0f;

            transform.position = _originalPosition + randomOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = _originalPosition;
    }
}
