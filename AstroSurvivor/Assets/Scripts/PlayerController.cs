using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController3D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Space Settings")]
    [SerializeField] private float dragCoefficient = 3f;

    private Rigidbody _rigidbody;
    private Camera _mainCamera;
    private Plane _movementPlane;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;

        _rigidbody.useGravity = false;
        _rigidbody.linearDamping = dragCoefficient;
        _rigidbody.angularDamping = 2f;
        _rigidbody.freezeRotation = true;

        _movementPlane = new Plane(Vector3.up, Vector3.zero);
    }

    private void FixedUpdate()
    {
        Move();
        RotateTowardsMouse();
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