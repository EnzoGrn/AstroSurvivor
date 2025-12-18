using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Space Settings")]
    [SerializeField] private float dragCoefficient = 3f;

    [Header("Barrel Roll")]
    [SerializeField] private float barrelRollDuration = 0.8f;
    [SerializeField] private float barrelRollCooldown = 1.5f;

    private Rigidbody _rigidbody;
    private Camera _mainCamera;
    private Plane _movementPlane;
    private bool _isBarrelRolling = false;
    private float _lastBarrelRollTime = -999f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
        _rigidbody.useGravity = false;
        _rigidbody.angularDamping = 2f;
        _rigidbody.freezeRotation = true;
        _movementPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void Update()
    {
        // Détection de la touche Espace pour le barrel roll
        if (Input.GetKeyDown(KeyCode.Space) && CanBarrelRoll())
        {
            StartCoroutine(PerformBarrelRoll());
        }
    }

    private void FixedUpdate()
    {
        if (!_isBarrelRolling)
        {
            Move();
            RotateTowardsMouse();
        }
    }

    private bool CanBarrelRoll()
    {
        return !_isBarrelRolling && (Time.time - _lastBarrelRollTime) >= barrelRollCooldown;
    }

    private IEnumerator PerformBarrelRoll()
    {
        _isBarrelRolling = true;
        _lastBarrelRollTime = Time.time;
        _rigidbody.freezeRotation = false;

        // Stocker la rotation Y initiale (rotation horizontale du joueur)
        float initialYRotation = transform.eulerAngles.y;

        float elapsed = 0f;

        while (elapsed < barrelRollDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / barrelRollDuration;

            // Calculer l'angle de roll (0 à 360 degrés)
            float rollAngle = progress * 360f;

            // Combiner la rotation Y (direction du joueur) avec le roll Z
            Quaternion targetRotation = Quaternion.Euler(0f, initialYRotation, rollAngle);
            _rigidbody.MoveRotation(targetRotation);

            yield return null;
        }

        // Réinitialiser à la rotation Y finale sans roll
        _rigidbody.MoveRotation(Quaternion.Euler(0f, initialYRotation, 0f));
        _rigidbody.freezeRotation = true;
        _isBarrelRolling = false;
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0.1f)
        {
            Vector3 targetVelocity = movement * maxSpeed;
            targetVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = Vector3.Lerp(_rigidbody.linearVelocity, targetVelocity, 0.5f);
        }
        else
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.x = Mathf.Lerp(velocity.x, 0f, dragCoefficient * Time.fixedDeltaTime);
            velocity.z = Mathf.Lerp(velocity.z, 0f, dragCoefficient * Time.fixedDeltaTime);
            _rigidbody.linearVelocity = velocity;
        }

        Vector3 vel = _rigidbody.linearVelocity;
        vel.y = 0f;
        if (vel.magnitude > maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
            vel.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = vel;
        }
    }

    private void RotateTowardsMouse()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (_movementPlane.Raycast(ray, out float distance))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(distance);
            Vector3 direction = mouseWorldPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion newRotation = Quaternion.Slerp(
                    _rigidbody.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
                _rigidbody.MoveRotation(newRotation);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Implémenter la logique de prise de dégâts ici
        Debug.Log($"Player took {damage} damage.");
    }
}