using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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

    public Transform shootPoint;
    public GameObject Bullet;
    public float fireRate = 1f;
    float nextTimeToFire = 0f;

    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();
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
        if (player.FindAction("Grapple").ReadValue<float>() > 0f && Time.time >= nextTimeToGrapple)
        {
            nextTimeToGrapple = Time.time + grappleCooldown;
            StartGrapple();
        }

        if (retracting)
        {
            playerRb.AddForce((target - transform.position) * grappleSpeed * Time.deltaTime);

            line.SetPosition(0, grappleShootPoint.position);

            if (Vector2.Distance(transform.position, target) < 1.2f/* || playerControls.PlayerMovement.Grapple.ReadValue<float>() > 0.1f*/ || grappleCollided == true)
            {
                retracting = false;
                line.enabled = false;
                grappleCollided = false;
                playerRb.gravityScale = 0.8f;
            } 
        }

        if (player.FindAction("Fire").ReadValue<float>() > 0f && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            GameObject bullet;
            bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.player = this.gameObject;
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
            bulletRb.velocity = player.FindAction("MoveGun").ReadValue<Vector2>() * bulletSpeed;
        }

        Rotate(player.FindAction("MoveGun").ReadValue<Vector2>(), gunHolder);
        Rotate(-player.FindAction("MoveGrapple").ReadValue<Vector2>(), grappleHolder);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (retracting && other.gameObject.tag != "Ground")
        {
            grappleCollided = true;
        }
    }

    private void StartGrapple()
    {
        grappleDirection = -grappleHolder.transform.right;

        RaycastHit2D hit = Physics2D.Raycast(grappleShootPoint.position, grappleDirection, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            target = hit.point;
            line.enabled = true;
            line.positionCount = 2;
            playerRb.gravityScale = 0f;

            StartCoroutine(Grapple());
        }
    }

    IEnumerator Grapple()
    {
        float t = 0;
        float time = 10;

        line.SetPosition(0, grappleShootPoint.position);
        line.SetPosition(1, grappleShootPoint.position); 

        Vector2 newPos;

        for (; t < time; t += grappleShootSpeed * Time.deltaTime) {
            newPos = Vector2.Lerp(grappleShootPoint.position, target, t / time);
            line.SetPosition(0, grappleShootPoint.position);
            line.SetPosition(1, newPos);
            yield return null;
        }
        
        line.SetPosition(1, target);
        retracting = true;
    }
}
