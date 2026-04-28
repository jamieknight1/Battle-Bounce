using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CpuCursor : MonoBehaviour
{
    public PlayerData playerData {get; private set;}

    public PlayerCard playerCard {get; private set;}

    public void InitializePlayerData(PlayerData data)
    {
        playerData = data;
    }

    public void InitializePlayerCard(PlayerCard card)
    {
        playerCard = card;
    }
}
