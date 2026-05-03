using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private ToBeNamed playerScript;
    [SerializeField] private float shootAtPlayerRange;
    [SerializeField] private float bulletDetectRange;

    private bool grappleShot = false;
    [SerializeField] private int shootGrappleChance;

    [SerializeField] private int chanceToLaunchSelf;
    private Vector3 wallToShootAt;

    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public bool teleportGrapplePressed;
    [HideInInspector] public Vector2 aimGun;
    [HideInInspector] public Vector2 aimGrapple;

    List<GameObject> activeNearbyBullets = new List<GameObject>();
    List<GameObject> activePlayers = new List<GameObject>();
    GameObject singlePlayer;
    void Awake()
    {
        playerScript = GetComponent<ToBeNamed>();

        foreach (GameObject activePlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            activePlayers.Add(activePlayer);
        }

        activePlayers.Remove(gameObject);

        if (activePlayers.Count == 1)
        {
            singlePlayer = activePlayers[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        firePressed = false;
        shootGrapplePressed = false;
        teleportGrapplePressed = false;

        int randomChanceToLaunchSelf = UnityEngine.Random.Range(1, chanceToLaunchSelf + 1);

        // if any active bullet is close enough, add it to the nearby bullets list
        foreach (GameObject bullet in GameObject.FindGameObjectsWithTag("Bullet"))
        {
            if (bullet.GetComponent<Bullet>().player != gameObject && Vector2.Distance(transform.position, bullet.transform.position) < bulletDetectRange)
            {
                activeNearbyBullets.Add(bullet);
            }
        }

        // constantly looping through the nearby bullets list and removing if they dont exist or arent nearby
        // foreach (GameObject bullet in activeNearbyBullets)
        // {
        //     if (bullet == null || Vector2.Distance(transform.position, bullet.transform.position) > bulletDetectRange)
        //     {
        //         activeNearbyBullets.Remove(bullet);
        //     }
        // }

        activeNearbyBullets.RemoveAll(bullet => bullet == null || Vector2.Distance(transform.position, bullet.transform.position) > bulletDetectRange);

        activePlayers.RemoveAll(player => player == null);

        //DODGING BULLETS AND PLAYER

        // if bullets are close, try to dodge otherwise, shoot at the bullets
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

            if (!grappleShot && wallToShootAt.magnitude == 0) { ShootAtSinglePlayer(closestBullet.transform.position); }
        }

        // randomly launches themself
        
        else if (randomChanceToLaunchSelf == 1 && Time.time >= playerScript.nextTimeToFire)
        {
            ShootAtSinglePlayer(wallToShootAt);
        }

        else if (singlePlayer != null)
        {
            
            // if the player is too close, get out their way
            if (Vector2.Distance(transform.position, singlePlayer.transform.position) < shootAtPlayerRange) { Dodge(); }

            // If player is far enough away, shoot at them
            else if (Vector2.Distance(transform.position, singlePlayer.transform.position) > shootAtPlayerRange && Time.time >= playerScript.nextTimeToFire) { ShootAtSinglePlayer(singlePlayer.transform.position); }

            else if (!grappleShot)
            {
                int chance = UnityEngine.Random.Range(1, shootGrappleChance + 1); 
                if (chance == 1) { ShootGrappleSinglePlayer(); }
            }
        }

        else if (singlePlayer == null)
        {
            GameObject closestPlayer = null;
            foreach (var player in activePlayers)
            {
                if (player != null)
                {
                    if (closestPlayer == null) { closestPlayer = player; }

                    else if (Vector2.Distance(transform.position, player.transform.position) < Vector2.Distance(transform.position, closestPlayer.transform.position)) { closestPlayer = player; }

                    if (Vector2.Distance(transform.position, player.transform.position) < shootAtPlayerRange) { Dodge(); }

                    else if (Vector2.Distance(transform.position, player.transform.position) > shootAtPlayerRange && Time.time >= playerScript.nextTimeToFire) { ShootAtSinglePlayer(player.transform.position); }
                }
            }

            // If player is far enough away, shoot at them
            

            if (!grappleShot)
            {
                int chance = UnityEngine.Random.Range(1, shootGrappleChance + 1); 
                if (chance == 1) { ShootGrappleMultiPlayer(closestPlayer); }
            }
        }
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

    // STATE MACHINE FUNCTIONS

    //SINGLE PLAYER FUNCTIONS

    private void ShootAtSinglePlayer(Vector3 playerToShootAt)
    {
        aimGun = playerToShootAt - transform.position;
        firePressed = true;
    }

    private void ShootAwayFromSinglePlayer(Vector3 shootAwayFrom)
    {
        aimGun = Quaternion.Euler(0f, 0f, -90f) * (shootAwayFrom - transform.position);
        firePressed = true;
    }

    private void ShootGrappleSinglePlayer()
    {
        aimGrapple = Quaternion.Euler(0f, 0f, 90f) * (singlePlayer.transform.position - transform.position);
        shootGrapplePressed = true;
        grappleShot = true;
    } 

    private void TeleportGrappleSinglePlayer()
    {
        teleportGrapplePressed = true;
        grappleShot = false;
    }

    private void Dodge()
    {
        if (!grappleShot && wallToShootAt.magnitude != 0)
        {
            ShootAtSinglePlayer(wallToShootAt);
        }

        else if (grappleShot && wallToShootAt.magnitude == 0)
        {
            TeleportGrappleSinglePlayer();
        }

        else if (grappleShot && wallToShootAt.magnitude != 0)
        {
            int shootOrGrapple = UnityEngine.Random.Range(1, 3);
            if (shootOrGrapple == 1) { ShootAtSinglePlayer(wallToShootAt); }
            else if (shootOrGrapple == 2) { TeleportGrappleSinglePlayer(); }
        }
    }

        //MULTIPLAYER FUNCTIONS
    
    private void ShootAtMultiPlayer()
    {
        
    }

    private void ShootGrappleMultiPlayer(GameObject player)
    {
        aimGrapple = Quaternion.Euler(0f, 0f, 90f) * (player.transform.position - transform.position);
        shootGrapplePressed = true;
        grappleShot = true;
    } 

    // GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, bulletDetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, shootAtPlayerRange);
    }
}
