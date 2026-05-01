using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Genesis : MonoBehaviour
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
    [SerializeField] Rigidbody2D playerRb;
    [SerializeField] private GameObject playerCharacter;
    bool canRotate = true;
    
    [SerializeField] Transform grappleShootPoint;
    [SerializeField] Transform grappleHolder;
    [HideInInspector] public bool retracting = false;
    bool grappleCollided = false;
    public float grappleCooldown;
    public float nextTimeToGrapple = 0.12f;
    Vector3 target;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 20f;
    LineRenderer line;
    Vector2 grappleDirection;
    [SerializeField] public LayerMask grapplableMask;
    [SerializeField] public float maxDistance = 10f;
    [SerializeField] GameObject tail;
    [SerializeField] Transform tailEnd;
    private Vector3 tailInitialLocalPos;

    public Transform shootPoint;
    public GameObject Bullet;
    public float fireRate = 1f;
    public float nextTimeToFire = 0.18f;

    [SerializeField] float maxIrisMovement = 1f;
    [SerializeField] float maxPupilMovement = 0.1f;
    Vector3 move;
    [SerializeField] GameObject iris;
    [SerializeField] GameObject pupil;

    Animator tailAnimator;
    Animator eyeAnimator;

    [SerializeField] GameManagement gameManagement;

    [HideInInspector] public bool firePressed;
    [HideInInspector] public bool shootGrapplePressed;
    [HideInInspector] public Vector2 aimGun;
    [HideInInspector] public Vector2 aimGrapple;

    [SerializeField] Health healthScript;

    void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        playerControls = new PlayerControls();

        tailAnimator = tail.GetComponent<Animator>();
        tailInitialLocalPos = tail.transform.localPosition;

        eyeAnimator = pupil.GetComponent<Animator>();

        gameManagement = GameObject.FindObjectOfType<GameManagement>();
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
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
            aimGrapple = player.FindAction("MoveGrapple").ReadValue<Vector2>();
            aimGun = player.FindAction("MoveGun").ReadValue<Vector2>();
        }

        else if (healthScript.cpu)
        {
            GenesisAI aiScript = GetComponent<GenesisAI>();
            
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
                if (tail.transform.localPosition == tailInitialLocalPos) { StartCoroutine(Animation("OnGrapple", 0.12f, tailAnimator, StartGrapple)); }
            }

            if (retracting)
            {
                playerRb.AddForce((target - transform.position) * grappleSpeed * Time.deltaTime);

                line.SetPosition(0, grappleShootPoint.position);

                if (Vector2.Distance(transform.position, target) < 1.2f || grappleCollided == true)
                {
                    retracting = false;
                    line.enabled = false;
                    grappleCollided = false;
                    playerRb.gravityScale = 0.8f;
                    tailAnimator.SetTrigger("StopGrapple");
                    tailAnimator.ResetTrigger("OnGrapple");
                    canRotate = true;
                    tail.transform.SetParent(grappleHolder);
                    tail.transform.localPosition = tailInitialLocalPos;
                    tail.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }

            if (firePressed && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                StartCoroutine(Animation("OnShoot", 0.18f, eyeAnimator, Shoot));
            }

            if (canRotate)
            {
                Rotate(-aimGrapple, grappleHolder);
            }
            MoveEye(iris, maxIrisMovement);
            MoveEye(pupil, maxPupilMovement);

        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {  
        if (retracting && other.gameObject.tag != "Ground")
        {
            grappleCollided = true;
        }

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
            line.enabled = true;
            line.positionCount = 2;
            playerRb.gravityScale = 0f;

            StartCoroutine(Grapple());
        }
        if (hit.collider == null)
        {
            tailAnimator.SetTrigger("StopGrapple");
            tailAnimator.ResetTrigger("OnGrapple");
        }
    }

    IEnumerator Grapple()
    {
        canRotate = false;
        tail.transform.SetParent(null);
        float t = 0;
        float time = 10;

        line.SetPosition(0, grappleShootPoint.position);
        line.SetPosition(1, grappleShootPoint.position);

        Vector2 newPos;

        for (; t < time; t += grappleShootSpeed * Time.deltaTime) {
            newPos = Vector2.Lerp(grappleShootPoint.position, target, t / time);
            line.SetPosition(0, grappleShootPoint.position);
            line.SetPosition(1, newPos);
            tail.transform.position += line.GetPosition(1) - tailEnd.position;
            yield return null;
        }
        
        line.SetPosition(1, tailEnd.position);
        retracting = true;
    }

    void MoveEye(GameObject eye, float maxEyeMovement)
    {   
        if (aimGun.magnitude == 0)
        {
            eye.transform.position = transform.position;
        }
        else
        {
            move = Vector2.ClampMagnitude(aimGun, maxEyeMovement);
            eye.transform.position = transform.position + move;
        }
    }

    IEnumerator Animation(string trigger, float timeToWait, Animator animator, Action function)
    {
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(timeToWait);
        function();
    }

    void Shoot()
    {
        if (aimGun.magnitude !=0)
        {
            GameObject bullet;
            bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletScript.player = this.gameObject;
            float bulletSpeed = bulletScript.speed;
            bulletRb.velocity = aimGun.normalized * bulletSpeed;
            Rotate(-aimGun, bullet.transform);

            
        }
        eyeAnimator.ResetTrigger("OnShoot");
        eyeAnimator.SetTrigger("StopShoot");
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