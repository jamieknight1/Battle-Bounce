using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public Transform shootPoint;
    public GameObject Bullet;
    public float fireRate = 1f;
    float nextTimeToFire = 0f;
    public PlayerControls playerControls;
    private InputActionAsset inputAsset;
    private InputActionMap player;

    void Awake()
    {
        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("PlayerMovement");
        //playerControls = new PlayerControls();
    }

    void OnEnable()
    {
        player.Enable();
    }

    void OnDisable()
    {
        player.Disable();
    }

    void Update()
    {
        if (player.FindAction("Fire").ReadValue<float>() > 0f && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            GameObject bullet;
            bullet = Instantiate(Bullet, shootPoint.position, shootPoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            float bulletSpeed = bullet.GetComponentInParent<Bullet>().speed;
            bulletRb.velocity = transform.right * bulletSpeed;
        }
    }
}