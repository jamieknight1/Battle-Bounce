using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTrail : MonoBehaviour
{
    [SerializeField] float damage;
    ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            particleSystem.trigger.AddCollider(player.GetComponent<Collider2D>());
        }
    }

    void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();

        int count = particleSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside, out var colliderData);

        for (int i = 0; i < count; i++)
        {
            Component hitCollider = colliderData.GetCollider(i, 0);

            if (hitCollider != null)
            {
                hitCollider.GetComponent<Health>().TakeDamage(damage);
            }
        }
    }
}
