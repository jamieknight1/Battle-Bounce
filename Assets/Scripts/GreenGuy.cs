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


    LineRenderer line;
    Vector2 grappleDirection;

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

    Vector3 target;

    public Transform gasShootPoint;
    public Transform sludgeShootPoint;
    public GameObject gasCloudBullet;
    public GameObject sludgeBombBullet;
    public float fireRate = 1f;
    float nextTimeToFire = 0f;

    [SerializeField] GameManagement gameManagement;

    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();

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
        if (!gameManagement.paused)
        {
            if (player.FindAction("Grapple").ReadValue<float>() > 0f && Time.time >= nextTimeToGrapple)
            {
                nextTimeToGrapple = Time.time + grappleCooldown;
                Shoot(gasCloudBullet, gasShootPoint, nextTimeToGrapple, gasShootPoint.right);
            }

            if (player.FindAction("Fire").ReadValue<float>() > 0f && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot(sludgeBombBullet, sludgeShootPoint, nextTimeToFire, -sludgeShootPoint.right);
            }

            Rotate(-player.FindAction("MoveGun").ReadValue<Vector2>(), gunHolder);
            Rotate(player.FindAction("MoveGrapple").ReadValue<Vector2>(), grappleHolder);
        }
    }

    private void Shoot(GameObject Bullet, Transform shootPoint, float nextTimeToShoot, Vector2 shootDirection)
    {
        GameObject bullet;
        bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.player = this.gameObject;
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
        bulletRb.velocity = shootDirection * bulletSpeed;
    }
}
