using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GreenGuy : MonoBehaviour
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

    public Transform gasShootPoint;
    public Transform sludgeShootPoint;
    public GameObject gasCloudBullet;
    public GameObject sludgeBombBullet;
    public float fireRate = 1f;
    float nextTimeToFire = 0f;

    [SerializeField] GameManagement gameManagement;

    [SerializeField] Health healthScript;
    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public Vector2 aimGun;
    [HideInInspector] public Vector2 aimGrapple;

    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();

        gameManagement = FindObjectOfType<GameManagement>();
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
            aimGrapple = player.FindAction("MoveGrapple").ReadValue<Vector2>();
            aimGun = player.FindAction("MoveGun").ReadValue<Vector2>();
        }

        else if (healthScript.cpu)
        {
            ToxicideAI aiScript = GetComponent<ToxicideAI>();
            
            firePressed = aiScript.firePressed;
            shootGrapplePressed = aiScript.shootGrapplePressed;
            aimGrapple = aiScript.aimGrapple;
            aimGun = aiScript.aimGun;
        }

        if (!gameManagement.paused)
        {
            if (shootGrapplePressed && Time.time >= nextTimeToGrapple)
            {
                nextTimeToGrapple = Time.time + grappleCooldown;
                Shoot(gasCloudBullet, gasShootPoint, gasShootPoint.right);
            }

            if (firePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot(sludgeBombBullet, sludgeShootPoint, -sludgeShootPoint.right);
            }

            Rotate(-aimGun, gunHolder);
            Rotate(aimGrapple, grappleHolder);
        }
    }

    private void Shoot(GameObject Bullet, Transform shootPoint, Vector2 shootDirection)
    {
        GameObject bullet;
        bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.player = gameObject;
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
        bulletRb.velocity = shootDirection * bulletSpeed;
    }
}
