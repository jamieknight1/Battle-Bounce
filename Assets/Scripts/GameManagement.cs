using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour
{
    [SerializeField] GameSetup gameSetup;

    [Space(10)]

    float startTime = 0f;
    float timer;
    bool timerEnded = false;
    [SerializeField] TMP_Text timerText;

    [Space(10)]

    string gamemode;
    PlayerInputManager playerInputManager;
    GameObject winningPlayer;
    public int winningPlayerNumber;

    private PlayerInput[] playerInputs;
    List<InputActionMap> playerInputActionMaps = new List<InputActionMap>();
    [HideInInspector] public bool paused = false;

    [SerializeField] private GameObject pauseMenu;
    public List<int> playerRankings = new List<int>();
    List<int> deadPlayers = new List<int>();

    void Awake()
    {
        //Destroy(GameObject.Find("Selection Screen Input Manager"));
        playerInputManager = FindFirstObjectByType<PlayerInputManager>();

        gameSetup.SetupVars();

        pauseMenu.SetActive(false);

        startTime = gameSetup.time;
        if (startTime != 0f) { timer = startTime; }

        gamemode = gameSetup.gamemode;

        if (startTime == 0f)
        {
            timerText.gameObject.SetActive(false);
        }

        playerInputs = FindObjectsOfType<PlayerInput>();

        foreach (PlayerInput pi in playerInputs)
        {
            playerInputActionMaps.Add(pi.actions.FindActionMap("PlayerMovement"));
        }
    }

    void Update()
    {
        if (startTime != 0f) { Timer(); }

        if (playerInputManager.playerCount <= 1 || timerEnded)
        {
            GameEnded();
        }

        Pause();
    }

    void Timer()
    {
        if (timer > 0f) { timer -= Time.deltaTime; }
        if (timer <= 0f) { timerEnded = true; }

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void GameEnded()
    {
        if (/*playerInputManager.playerCount == 1*/ GameObject.FindGameObjectsWithTag("Player").Length == 1)
        {
            winningPlayer = GameObject.FindWithTag("Player");
            for (int i = deadPlayers.Count - 1; i >= 0; i--)
            {
                playerRankings.Add(deadPlayers[i]);
            }

            playerRankings.Insert(winningPlayerNumber, 0);
        }

        if (timerEnded)
        {
            GameObject[] activePlayerList = GameObject.FindGameObjectsWithTag("Player");

            Dictionary<GameObject, (int lives, float health)> playerStats = new Dictionary<GameObject, (int lives, float health)>();

            foreach (GameObject player in activePlayerList)
            {
                Health playerHealth = player.GetComponent<Health>();
                playerStats[player] = (playerHealth.lives, playerHealth.currentHealth);
            }

            // int maxLives = int.MinValue;
            // List<GameObject> potentialWinners = new List<GameObject>();

            // foreach (var player in playerStats)
            // {
            //     int lives = player.Value.lives;

            //     if (lives > maxLives)
            //     {
            //         maxLives = lives;
            //         potentialWinners.Clear();
            //         potentialWinners.Add(player.Key);
            //     }

            //     else if (lives == maxLives)
            //     {
            //         potentialWinners.Add(player.Key);
            //     }
            // }

            // winningPlayer = potentialWinners[0];

            // if (potentialWinners.Count > 1)
            // {
            //     float maxHealth = playerStats[winningPlayer].health;

            //     foreach (GameObject player in potentialWinners)
            //     {
            //         if (playerStats[player].health > maxHealth)
            //         {
            //             maxHealth = playerStats[player].health;
            //             winningPlayer = player;
            //         }
            //     }
            // }

            List<GameObject> rankedPlayers = playerStats.OrderByDescending(p => p.Value.lives).ThenByDescending(p => p.Value.health).Select(p => p.Key).ToList();

            winningPlayer = rankedPlayers[0];

            // for (int i = 0; i < rankedPlayers.Count; i++)
            // {
            //     var stats = playerStats[rankedPlayers[i]];
            //     //Debug.Log($"{i + 1}: {rankedPlayers[i].name} - Lives: {stats.lives}, Health: {stats.health}");
            //     //playerRankings.Add(rankedPlayers[i].GetComponent<PlayerInput>().playerIndex);
            // }
            
            for (int i = 0; i < rankedPlayers.Count; i++)
            {
                playerRankings.Add(rankedPlayers[i].GetComponent<PlayerInput>().playerIndex);
            }

            for (int i = deadPlayers.Count - 1; i >= 0; i--)
            {
                playerRankings.Add(deadPlayers[i]);
            }
        }

        for (int i = 0; i < playerRankings.Count; i++)
        {
            Debug.Log(playerRankings[i]);
        }
        winningPlayerNumber = winningPlayer.GetComponent<PlayerInput>().playerIndex;
        gameSetup.winningPlayerNumber = winningPlayerNumber + 1;
        gameSetup.playerRankings = playerRankings;
        SceneManager.LoadScene("GameEndScreen");
    }

    private void Pause()
    {
        foreach (InputActionMap inputAction in playerInputActionMaps)
        {
            if (inputAction.FindAction("Pause").triggered)
            {
                if (paused)
                {
                    Time.timeScale = 1;
                    paused = false;
                    pauseMenu.SetActive(false);
                    break;
                }

                if (!paused)
                {
                    Time.timeScale = 0;
                    paused = true;
                    pauseMenu.SetActive(true);
                    break;
                }
            }
        }
    }

    public void PlayerDeath(GameObject player)
    {
        deadPlayers.Add(player.GetComponent<PlayerInput>().playerIndex);
    }

    public void ButtonPause()
    {
        if (paused)
        {
            Time.timeScale = 1;
            paused = false;
            pauseMenu.SetActive(false);
        }

        else if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            pauseMenu.SetActive(true);
        }
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ButtonPause();
    }

    public void QuitButton()
    {

    }
}