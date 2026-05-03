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

    public float grappleCooldown;
    float nextTimeToGrapple = 0f;
    [SerializeField] private bool grappleShot = false;
    [SerializeField] private Transform grapple;
    Rigidbody2D grappleShootPointRb;
    private Vector3 originalGrapplePos;
    [SerializeField] private float grappleShootForce;
    public bool grappleHit = false;
    [SerializeField] ToBeNamedGrapple grappleScript;

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

        gameManagement = FindObjectOfType<GameManagement>();
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();

        originalGrapplePos = grappleShootPoint.transform.localPosition;
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
                grappleShot = true;
            }

            if (teleportGrapplePressed && grappleShot && grappleShootPointRb.velocity.magnitude == 0f)
            {
                nextTimeToGrapple = Time.time + grappleCooldown;
                TeleportGrapple();
            }

            if (grappleShot && !grappleHit)
            {
                Grapple();
            }

            if (firePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                GameObject bullet;
                bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.player = gameObject;
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
                bulletRb.velocity = shootPoint.right * bulletSpeed;
            }

            Rotate(aimGun, gunHolder);
            Rotate(-aimGrapple, grappleHolder);
        }
    }

    // private void StartGrapple()
    // {
    //     grappleDirection = -grappleHolder.transform.right;
    //     RaycastHit2D hit = Physics2D.Raycast(grappleShootPoint.position, grappleDirection, maxDistance, grapplableMask);
    //     if (hit.collider != null)
    //     {
    //         target = hit.point;
    //         grappleShot = true;
    //         StartCoroutine(Grappling());
    //     }
    // }

    // IEnumerator Grappling()
    // {
    //     grappleShootPoint.SetParent(null);
    //     float t = 0;
    //     float time = 10;
    //     Vector2 newPos;
    //     Vector2 grappleStartPos = grapple.position;
    //     for (; t <= time; t += grappleShootSpeed * Time.deltaTime)
    //     {
    //         newPos = Vector2.Lerp(grappleStartPos, target, t/time);
    //         grappleShootPoint.position = newPos;
    //         yield return null;
    //     }
    // }

    private void TeleportGrapple()
    {
        grappleShot = false;
        grappleHit = false;
        transform.position = grappleShootPoint.position;
        grappleShootPoint.SetParent(grapple);
        grappleShootPoint.localRotation = Quaternion.Euler(0f,0f,0f);
        grappleShootPoint.localPosition = originalGrapplePos;
    }

    private void Grapple()
    {
        grappleShootPoint.SetParent(null);
        grappleScript.ShootGrapple(grappleShootForce);
    }
}
