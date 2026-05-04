using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;
using System;

public class Health : MonoBehaviour
{
    public bool cpu = false;
    private float maxHealth = 100f;
    public float currentHealth;
    public GameObject healthBarPf;
    GameObject healthBar;
    HealthBar healthBarScript;
    RectTransform healthBarTransform;
    string playerNumber;
    public PlayerInputManager playerInputManager;
    public int lives;
    [SerializeField] GameSetup gameSetup;
    GameManagement gameManagement;

    public static event Action LostLife; 


    // Start is called before the first frame update
    void Awake()
    {
        gameManagement = FindObjectOfType<GameManagement>();
        lives = gameSetup.playerLives;
        playerInputManager = FindObjectOfType<PlayerInputManager>();
        playerNumber = playerInputManager.playerCount.ToString();

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.StartsWith("Map_"))
        {
            GameObject healthBar = Instantiate(healthBarPf);
            healthBarScript = healthBar.GetComponent<HealthBar>();
            healthBarTransform = healthBar.GetComponent<RectTransform>();
            currentHealth = maxHealth;
            healthBarScript.SetMaxHealth(maxHealth);
            healthBarTransform.localPosition = GameObject.Find("P" + playerNumber.ToString() + " Health Bar Spawn").GetComponent<RectTransform>().localPosition;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBarScript.SetHealth(currentHealth);
        if (currentHealth <= 0 && lives == 1)
        {
            lives -= 1;
            gameManagement.PlayerDeath(gameObject);
            Destroy(gameObject);
            LostLife?.Invoke();
        }

        else if (currentHealth <= 0 && lives > 1)
        {
            currentHealth = maxHealth;
            lives -= 1;
            LostLife?.Invoke();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(other.gameObject.GetComponent<Bullet>().damage);
        }
    }
}
