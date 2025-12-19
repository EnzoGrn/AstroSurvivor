using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI")]
    public Image healthFill;
    public Vector3 offset = new Vector3(0f, 2f, 0f);

    private Camera cam;

    void Start()
    {
        currentHealth = maxHealth;
        cam = Camera.main;
    }

    void Update()
    {
        // Toujours regarder la caméra
        if (cam != null)
            transform.LookAt(transform.position + cam.transform.forward);

        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        if (healthFill != null)
        {
            float percent = currentHealth / maxHealth;
            healthFill.fillAmount = percent;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
