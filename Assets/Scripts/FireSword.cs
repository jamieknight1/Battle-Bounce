using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSword : MonoBehaviour
{
    [SerializeField] GameObject fireGuy;
    float swordDamage;
    void Awake()
    {
        swordDamage = GetComponentInParent<FireGuy>().swordDamage;
    }
    // [SerializeField] GameObject playerGo;

    // public Rigidbody2D playerRb;
    // Rigidbody2D swordRb;

    // [SerializeField] float force;

    // void Awake()
    // {
    //     swordRb = GetComponent<Rigidbody2D>();
    // }

    // void OnCollisionEnter2D(Collision2D other)
    // {
    //     Debug.Log("hit");

    //     Vector2 collisionDistance;
    //     ContactPoint2D contactPoint;
    //     Vector2 collisionPos;

    //     contactPoint = other.GetContact(0);
    //     collisionPos = contactPoint.point;

    //     Debug.Log(collisionPos);

    //     //collisionDistance = new Vector2(collisionPos.x - playerGo.transform.position.x, collisionPos.y - playerGo.transform.position.y);

    //     collisionDistance = transform.position - playerGo.transform.position;
    //     playerRb.AddForce(-collisionDistance * force);
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject != fireGuy)
        {
            Health playerHealth = other.gameObject.GetComponent<Health>();
            playerHealth.TakeDamage(swordDamage);
        }
    }
}
