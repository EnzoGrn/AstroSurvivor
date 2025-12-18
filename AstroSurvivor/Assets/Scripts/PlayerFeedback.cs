using UnityEngine;

public class PlayerFeedback : MonoBehaviour
{
    [SerializeField] private CameraShake cameraShake;

    public void OnPlayerHit()
    {
        cameraShake?.Shake(0.12f, 0.25f);
        // Plus tard : flash UI rouge, son, etc.
    }

    public void OnPlayerDeath()
    {
        cameraShake?.Shake(0.3f, 0.5f);
        // Slow motion, fade, etc.
    }
}
