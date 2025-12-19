using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private void Start()
    {
        // Si l'objet a un ParticleSystem
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Détruire après la durée + durée max des particules
            Destroy(gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            // Sinon, détruire après 2 secondes par défaut
            Destroy(gameObject, 2f);
        }
    }
}
