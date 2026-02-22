using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public float bulletTime = 1f;
    public float explosionForce = 1000000f;
    public GameObject player;
    public string playerNumber;
    public float maxKnockbackDistance = 10f;
    [SerializeField] private float maxDamage = 10f;
    public float damage;
    [SerializeField] ParticleSystem onCollisionParticle;
    [SerializeField] ParticleSystem onMoveParticlePf;

    void Start()
    {
        Destroy(gameObject, bulletTime);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.GetComponent<Collider2D>());

        if (onMoveParticlePf != null)
        {
            ParticleSystem onMoveParticle = Instantiate(onMoveParticlePf, transform.position, transform.rotation);
            onMoveParticle.transform.SetParent(this.transform);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == player)
        {
            return;
        }
        else if (other.gameObject.tag == "Player")
        {
            Knockback();
            SpawnParticle(onCollisionParticle);
            Destroy(gameObject);
        }
        else
        {
            Knockback();
            SpawnParticle(onCollisionParticle);
            Destroy(gameObject);
        }
    }

    public void Knockback()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            Rigidbody2D playerRb = players[i].GetComponent<Rigidbody2D>();
            float distance = Vector3.Distance(players[i].transform.position, transform.position);
            Vector3 knockbackDir = (players[i].transform.position - transform.position).normalized;

            if (distance < maxKnockbackDistance)
            {
                float force = maxKnockbackDistance / distance * explosionForce;
                playerRb.AddForce(knockbackDir * force);
                damage = maxDamage * (maxKnockbackDistance / distance);
            }
        }
    }

    private void SpawnParticle(ParticleSystem particle)
    {
        if (particle != null)
        {
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }
}
