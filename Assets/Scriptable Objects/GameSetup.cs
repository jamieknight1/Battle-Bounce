using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSetup", menuName = "Scriptable Objects/GameSetup")]
public class GameSetup : ScriptableObject
{
    public int playerLives;
    public int rulesetTime;
    public int rulesetGamemode;
    
    public float time;
    public string gamemode;

    public float[] seconds = { 60f, 120f, 180f, 240f, 320f, 600f, 0f };
    public string[] rulesets = { "Life", "Deal Damage" };

    public int winningPlayerNumber;

    public string map;
    public List<int> playerRankings;

    public void SetupVars()
    {
        time = seconds[rulesetTime];
        gamemode = rulesets[rulesetGamemode];
    }
}
