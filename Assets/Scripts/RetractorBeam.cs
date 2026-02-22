using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetractorBeam : MonoBehaviour
{
    [SerializeField] float retractSpeed;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();
            playerRb.AddForce(Vector2.up * Time.deltaTime * retractSpeed);
        }
    }
}
