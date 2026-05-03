using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToBeNamedGrapple : MonoBehaviour
{
    [SerializeField] ToBeNamed playerScript;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Grappleable") && transform.parent == null)
        {
            playerScript.grappleHit = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerObjects") && transform.parent == null && playerScript.grappleHit == false)
        {
            playerScript.grappleHit = true;
            transform.SetParent(collision.transform);
        }
    }

    public void ShootGrapple(float force)
    {
        transform.Translate(-transform.right * force * Time.deltaTime, Space.World);
    }
}
