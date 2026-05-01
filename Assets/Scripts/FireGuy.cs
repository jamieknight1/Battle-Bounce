using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireGuy : MonoBehaviour
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

    //LineRenderer line;
    Vector2 grappleDirection;
    Quaternion grappleRotation;

    [SerializeField] LayerMask grapplableMask;
    [SerializeField] public float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] Transform grappleShootPoint;
    [SerializeField] Transform grappleHolder;
    [SerializeField] private GameObject playerCharacter;

    [HideInInspector] public bool retracting = false;
    bool grappleCollided = false;

    public float grappleCooldown;
    public float nextTimeToGrapple {get; private set;}

    Vector3 target;

    public Transform shootPoint;
    public GameObject Bullet;
    public float fireRate = 1f;
    public float nextTimeToFire {get; private set;}

    [SerializeField] GameObject flameTrailPf;
    float nextTimeToSpawnFlameTrail = 0f;
    [SerializeField] float flameTrailCooldown = 0.01f;

    Rigidbody2D swordRb;
    [SerializeField] Transform swordTransform;
    bool swordCollided = false;
    [SerializeField] float swordForce;
    [SerializeField] Collider2D swordCollider;
    [SerializeField] public float swordDamage = 10f;
    [SerializeField] private ParticleSystem flameTrail;

    [SerializeField] GameManagement gameManagement;

    [SerializeField] Health healthScript;
    [SerializeField] InferknightAI aiScript;
    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public Vector2 aimGun;

    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();

        swordRb = GetComponentInChildren<Rigidbody2D>();
        swordCollider.enabled = false;

        gameManagement = GameObject.FindObjectOfType<GameManagement>();

        nextTimeToFire = 0.5f;
        nextTimeToGrapple = 0.5f;
    }

    private void Start()
    {
        //line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        player.Enable();
    }

    void OnDisable()
    {
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
            aimGun = player.FindAction("MoveGun").ReadValue<Vector2>();
        }

        else if (healthScript.cpu)
        {
            firePressed = aiScript.firePressed;
            shootGrapplePressed = aiScript.shootGrapplePressed;
            aimGun = aiScript.aimGun;
        }

        if (!gameManagement.paused)
        {
            if (shootGrapplePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + grappleCooldown;
                StartGrapple();
            }

            if (retracting)
            {
                swordCollider.enabled = false;
                playerRb.AddForce((target - transform.position) * grappleSpeed * Time.deltaTime);

                //line.SetPosition(0, grappleShootPoint.position);

                if (Vector2.Distance(transform.position, target) < 1.2f || grappleCollided == true)
                {
                    flameTrail.Stop();
                    retracting = false;
                    //line.enabled = false;
                    grappleCollided = false;
                    playerRb.gravityScale = 0.8f;

                    swordTransform.SetParent(grappleHolder);
                    swordTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    swordTransform.localPosition = new Vector2(46.5f, 0f);
                }
            }

            if (firePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                GameObject bullet;
                bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.player = this.gameObject;
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
                bulletRb.velocity = gunHolder.right * bulletSpeed;

                if (swordCollided == true) { playerRb.AddForce((transform.position - swordTransform.position) * swordForce); swordCollided = false; }

                if (aimGun.x > 0) { StartCoroutine(RotateSword(0.5f, -360.0f)); }
                if (aimGun.x < 0) { StartCoroutine(RotateSword(0.5f, 360.0f)); }
            }

            Rotate(aimGun, gunHolder);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (retracting && other.gameObject.tag == "Ground")
        {
            grappleCollided = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        swordCollided = true;

        if (other.gameObject.CompareTag("Player") && other.gameObject != this)
        {
            Health playerHealth = other.gameObject.GetComponent<Health>();
            playerHealth.TakeDamage(swordDamage);
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }

    private void StartGrapple()
    {
        grappleDirection = grappleHolder.transform.right;
        grappleRotation = grappleHolder.rotation;

        RaycastHit2D hit = Physics2D.Raycast(grappleShootPoint.position, grappleDirection, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            target = hit.point;
            //line.enabled = true;
            //line.positionCount = 2;
            playerRb.gravityScale = 0f;

            StartCoroutine(Grapple());
        }
    }

    IEnumerator Grapple()
    {
        if (aimGun.x > 0) { yield return StartCoroutine(RotateSword(0.5f, -360.0f)); }
        else if (aimGun.x < 0) { yield return StartCoroutine(RotateSword(0.5f, 360.0f)); }

        flameTrail.Play();

        swordCollider.enabled = true;

        swordTransform.SetParent(null);
        swordTransform.rotation = grappleRotation;

        float t = 0;
        float time = 10;

        // line.SetPosition(0, grappleShootPoint.position);
        // line.SetPosition(1, grappleShootPoint.position);

        Vector2 newPos;

        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(grappleShootPoint.position, target, t / time);
            swordTransform.position = newPos;
            yield return null;
        }

        //line.SetPosition(1, target);
        retracting = true;
    }

    IEnumerator RotateSword(float duration, float angle)
    {
        swordCollider.enabled = true;
        float startRotation = gunHolder.eulerAngles.z;
        float endRotation = startRotation + angle;
        float t = 0.0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            gunHolder.eulerAngles = new Vector3(gunHolder.eulerAngles.x, gunHolder.eulerAngles.y, zRotation);
            yield return null;
        }
        swordCollider.enabled = false;
    }

    void SpawnFlameTrail()
    {
        if (retracting)
        {
            flameTrail.Play();
        }

        if (!retracting)
        {
            //flameTrail.Stop();
        }

        // if (Time.time >= nextTimeToSpawnFlameTrail)
        // {
        //     nextTimeToSpawnFlameTrail = Time.time + flameTrailCooldown;
        //     GameObject flameTrail = Instantiate(flameTrailPf);
        //     Destroy(flameTrail, 5f);
        //     flameTrail.transform.position = transform.position;
        //     BoxCollider2D flameTrailCollider = flameTrail.GetComponent<BoxCollider2D>();
        //     Physics2D.IgnoreCollision(flameTrailCollider, GetComponent<Collider2D>());
        // }
    }
}