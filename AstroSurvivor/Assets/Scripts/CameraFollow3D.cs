using UnityEngine;

public class CameraFollow3D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField] private float height = 20f;
    [SerializeField] private float distance = -5f;
    [SerializeField] private float angle = 60f;

    [Header("Follow Settings")]
    [SerializeField] private float followSmoothness = 8f;
    [SerializeField] private float rotationSmoothness = 5f;

    [Header("Options")]
    [SerializeField] private bool followRotation = false;

    private void LateUpdate()
    {
        target = GameObject.FindWithTag("Player").transform;

        if (target == null)
            return;
        // Calculer la position désirée de la caméra
        Vector3 offset = Vector3.back * distance + Vector3.up * height;

        // Si on suit la rotation du joueur
        if (followRotation)
        {
            offset = target.rotation * offset;
        }

        Vector3 desiredPosition = target.position + offset;

        // Suivre smoothement la position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmoothness * Time.deltaTime
        );

        // Orienter la caméra vers le joueur
        Quaternion desiredRotation = Quaternion.Euler(angle, target.eulerAngles.y, 0f);

        if (followRotation)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRotation,
                rotationSmoothness * Time.deltaTime
            );
        }
        else
        {
            transform.rotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }

    public void DezoomForBarrelRoll()
    {
        height += 20f;
        distance += 20f;
    }

    public void RezoomAfterBarrelRoll()
    {
        height -= 20f;
        distance -= 20f;
    }
}