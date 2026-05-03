using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ToBeNamed : MonoBehaviour
{
    public Transform gunHolder;
    Vector3 playerPos;
    public PlayerControls playerControls;
    private Vector2 direction;
    public PlayerInput playerInput;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    public string playerNumber;
    public PlayerInputManager playerInputManager;

    LineRenderer line;
    Vector2 grappleDirection;
    float elapsedTime;

    [SerializeField] LayerMask grapplableMask;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Transform grappleShootPoint;
    [SerializeField] Transform grappleHolder;
    [SerializeField] private GameObject playerCharacter;

    [HideInInspector] public bool retracting = false;
    bool grappleCollided = false;

    public float grappleCooldown;
    float nextTimeToGrapple = 0f;
    [SerializeField] private bool grappleShot = false;
    [SerializeField] private Transform grapple;
    Rigidbody2D grappleShootPointRb;

    Vector3 target;

    public Transform shootPoint;
    public GameObject Bullet;
    public float fireRate = 1f;
    public float nextTimeToFire = 0f;

    [SerializeField] GameManagement gameManagement;

    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public bool teleportGrapplePressed;
    [HideInInspector] public Vector2 aimGun;
    [HideInInspector] public Vector2 aimGrapple;

    Health healthScript;

    void Awake()
    {
        healthScript = GetComponent<Health>();
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();
        grappleShootPointRb = grappleShootPoint.GetComponent<Rigidbody2D>();

        gameManagement = GameObject.FindObjectOfType<GameManagement>();
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        //playerControls.Enable();
        player.Enable();
    }

    void OnDisable()
    {
        //playerControls.Disable();
        player.Disable();
    }

    private void Rotate(Vector2 dir, Transform transform)
    {
        direction = dir;
        playerPos = transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (direction.magnitude != 0)
        {
            transform.rotation = rotation;
        }
    }

    void Update()
    {
        if (!healthScript.cpu)
        {
            firePressed = player.FindAction("Fire").ReadValue<float>() > 0f;
            shootGrapplePressed = player.FindAction("Grapple").ReadValue<float>() > 0f;
            teleportGrapplePressed = player.FindAction("TeleportGrapple").ReadValue<float>() > 0f;
            aimGrapple = player.FindAction("MoveGrapple").ReadValue<Vector2>();
            aimGun = player.FindAction("MoveGun").ReadValue<Vector2>();
        }

        else if (healthScript.cpu)
        {
            EnemyAI aiScript = GetComponent<EnemyAI>();
            
            firePressed = aiScript.firePressed;
            shootGrapplePressed = aiScript.shootGrapplePressed;
            teleportGrapplePressed = aiScript.teleportGrapplePressed;
            aimGrapple = aiScript.aimGrapple;
            aimGun = aiScript.aimGun;
        }

        if (!gameManagement.paused)
        {
            if (shootGrapplePressed && Time.time >= nextTimeToGrapple && !grappleShot)
            {
                StartGrapple();
            }

            if (teleportGrapplePressed && Time.time >= nextTimeToGrapple && grappleShot && grappleShootPointRb.velocity.magnitude == 0f)
            {
                nextTimeToGrapple = Time.time + grappleCooldown;
                Grapple();
            }

            // if (retracting)
            // {
            //     playerRb.AddForce((target - transform.position) * grappleSpeed * Time.deltaTime);

            //     //line.SetPosition(0, grappleShootPoint.position);

            //     if (Vector2.Distance(transform.position, target) < 1.2f/* || playerControls.PlayerMovement.Grapple.ReadValue<float>() > 0.1f*/ || grappleCollided == true)
            //     {
            //         retracting = false;
            //         line.enabled = false;
            //         grappleCollided = false;
            //         playerRb.gravityScale = 0.8f;
            //     }
            // }

            if (firePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                GameObject bullet;
                bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.player = this.gameObject;
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
                bulletRb.velocity = shootPoint.right * bulletSpeed;
            }

            Rotate(aimGun, gunHolder);
            Rotate(-aimGrapple, grappleHolder);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // if (retracting && other.gameObject.tag != "Ground")
        // {
        //     grappleCollided = true;
        // }

        // if (other.gameObject.CompareTag("Bullet"))
        // {
        //     TakeDamage(other.gameObject.GetComponent<Bullet>().damage);
        // }
    }

    private void StartGrapple()
    {
        grappleDirection = -grappleHolder.transform.right;
        RaycastHit2D hit = Physics2D.Raycast(grappleShootPoint.position, grappleDirection, maxDistance, grapplableMask);
        if (hit.collider != null)
        {
            target = hit.point;
            grappleShot = true;
            StartCoroutine(Grappling());
        }

        
        
        //Rigidbody2D grappleShootPointRb = grappleShootPoint.GetComponent<Rigidbody2D>();
        //grappleShootPointRb.MovePosition(grappleShootPoint.transform.position + target * grappleShootSpeed * Time.deltaTime);
        
        // grappleShootPoint.transform.position = Vector2.Lerp(grappleShootPoint.transform.position, target, elapsedTime/grappleShootSpeed);
        //Vector2 startPos = grappleShootPoint.transform.position;
        //grappleShootPoint.transform.position = Vector2.Lerp(startPos, target, );

    }

    IEnumerator Grappling()
    {
        grappleShootPoint.SetParent(null);
        float t = 0;
        float time = 10;
        Vector2 newPos;
        Vector2 grappleStartPos = grapple.position;
        for (; t <= time; t += grappleShootSpeed * Time.deltaTime)
        {
            //if (!player.FindAction("TeleportGrapple").triggered)
            //{
                newPos = Vector2.Lerp(grappleStartPos, target, t/time);
                grappleShootPoint.position = newPos;
                yield return null;
            //}
        }
    }

    private void Grapple()
    {
        StopCoroutine(Grappling());
        grappleShot = false;
        transform.position = grappleShootPoint.position;
        grappleShootPoint.SetParent(grapple);
        grappleShootPoint.localRotation = Quaternion.Euler(0f,0f,0f);
        grappleShootPoint.localPosition = new Vector2(-8.5f,0f);
    }

    // public void TakeDamage(float damage)
    // {
    //     currentHealth -= damage;
    //     healthBarScript.SetHealth(currentHealth);
    //     if(currentHealth <= 0)
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}
