using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 _direction;
    private float _speed;
    private int _damage;
    private float _lifetime = 3f;
    private float _timer;

    public void Fire(Vector3 direction, float speed, int damage)
    {
        _direction = direction.normalized;
        _speed = speed;
        _damage = damage;
        _timer = 0f;
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
        if (other.GetComponent<EnemyHealth>() is EnemyHealth enemy)
        {
            enemy.TakeDamage(_damage);
            Disable();
        }
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
