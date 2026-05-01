using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenesisAI : MonoBehaviour
{
    private Genesis playerScript;
    [SerializeField] private float shootAtPlayerRange;
    [SerializeField] private float bulletDetectRange;

    private bool grappleShot = false;

    [SerializeField] private int chanceToLaunchSelf;
    private Vector3 wallToShootAt;

    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public Vector2 aimGun;
    [HideInInspector] public Vector2 aimGrapple;

    List<GameObject> activeNearbyBullets = new List<GameObject>();
    List<GameObject> activePlayers = new List<GameObject>();
    GameObject closestPlayer;
    
    void Awake()
    {
        playerScript = GetComponent<Genesis>();

        foreach (GameObject activePlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            activePlayers.Add(activePlayer);
        }

        activePlayers.Remove(gameObject);

        if (activePlayers.Count == 1)
        {
            closestPlayer = activePlayers[0];
        }
    }

    void Update()
    {
        firePressed = false;
        shootGrapplePressed = false;

        int randomChanceToLaunchSelf = Random.Range(1, chanceToLaunchSelf + 1);

        //Finding the closest player
        if (activePlayers.Count > 1)
        {
            foreach (var player in activePlayers)
            {
                if (closestPlayer == null) { closestPlayer = player; }
                else if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, closestPlayer.transform.position)) { closestPlayer = player; }
            }
        }

        else if (activePlayers.Count == 1) { closestPlayer = activePlayers[0]; }

        //Finding nearby bullets
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            if (bullet.GetComponent<Bullet>().player != gameObject && Vector2.Distance(transform.position, bullet.transform.position) < bulletDetectRange)
            {
                activeNearbyBullets.Add(bullet);
            }
        }

            //Clearing any bullets that arent in range
        foreach (GameObject bullet in activeNearbyBullets)
        {
            if (bullet == null || Vector2.Distance(transform.position, bullet.transform.position) > bulletDetectRange)
            {
                activeNearbyBullets.Remove(bullet);
            }
        }

        //Finding the closest bullet
        if (activeNearbyBullets.Count > 0)
        {
            GameObject closestBullet = null;

            for (int i = 0; i < activeNearbyBullets.Count; i++)
            {
                if (closestBullet == null)
                {
                    closestBullet = activeNearbyBullets[i];
                }

                else if (Vector2.Distance(transform.position, activeNearbyBullets[i].transform.position) < Vector2.Distance(transform.position, closestBullet.transform.position))
                {
                    closestBullet = activeNearbyBullets[i];
                }
            }

            Dodge();

            if (wallToShootAt.magnitude == 0) { ShootAt(closestBullet.transform.position); }
        }

        else if (randomChanceToLaunchSelf == 1 && Time.time >= playerScript.nextTimeToFire)
        {
            ShootAt(wallToShootAt);
        }

        // if the player is too close, get away
        else if (Vector2.Distance(transform.position, closestPlayer.transform.position) < shootAtPlayerRange) { Dodge(); }

        // If player is far away, shoot at them
        else if (Vector2.Distance(transform.position, closestPlayer.transform.position) > shootAtPlayerRange && Time.time >= playerScript.nextTimeToFire) { ShootAt(closestPlayer.transform.position); }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ContactPoint2D wallContact;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.collider.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
            {
                wallContact = contact;
                wallToShootAt = wallContact.point;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        wallToShootAt = Vector3.zero;
    }

    private void Dodge()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(0f, 0f, 90f) * (closestPlayer.transform.position - transform.position), playerScript.maxDistance, playerScript.grapplableMask);
        if (wallToShootAt.magnitude != 0 && hit.collider != null)
        {
            int randomChance = Random.Range(1, 3);
            if (randomChance == 1) { ShootAt(wallToShootAt); }
            if (randomChance == 2) { ShootGrappleAway(closestPlayer.transform.position); }
        }

        if (wallToShootAt.magnitude != 0 && hit.collider == null)
        {
            ShootAt(wallToShootAt);
        }

        else if (wallToShootAt.magnitude == 0 && hit.collider == null)
        {
            ShootGrappleAway(closestPlayer.transform.position);
        }

        else if (wallToShootAt.magnitude == 0 && hit.collider != null)
        {
            ShootAt(closestPlayer.transform.position);
        }
    }

    private void ShootAt(Vector3 playerToShootAt)
    {
        aimGun = playerToShootAt - transform.position;
        firePressed = true;
    }

    private void ShootGrappleTowards(Vector3 grappleTowards)
    {
        aimGun = grappleTowards - transform.position;
        shootGrapplePressed = true;
    }

    private void ShootGrappleAway(Vector3 grappleAwayFrom)
    {
        aimGun = Quaternion.Euler(0f, 0f, 90f) * (grappleAwayFrom - transform.position);
        shootGrapplePressed = true;
    }
}
