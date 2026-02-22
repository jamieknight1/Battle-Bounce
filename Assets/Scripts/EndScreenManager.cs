using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameSetup gameSetup;
    [SerializeField] Transform[] endCards;
    [SerializeField] GameObject[] endCardBases;
    Dictionary<Transform, (Vector2 startPos, Vector2 endPos)> endCardsPos = new Dictionary<Transform, (Vector2, Vector2)>();

    float elapsedTime;
    [SerializeField] float duration = 0.5f;

    void Awake()
    {
        text.text = "Player " + gameSetup.winningPlayerNumber.ToString() + " Wins!";
        EndCards();
    }

    void EndCards()
    {
        for (int i = 0; i < gameSetup.playerRankings.Count; i++)
        {
            endCardsPos.Add(endCards[gameSetup.playerRankings[i]], (endCards[gameSetup.playerRankings[i]].localPosition, new Vector2(endCards[gameSetup.playerRankings[i]].localPosition.x, -3f * i / (gameSetup.playerRankings.Count - 1f))));
            Debug.Log("Index: " + i.ToString() + " Player Count: " + gameSetup.playerRankings.Count.ToString());
        }

        for (int i = 3; i > gameSetup.playerRankings.Count - 1; i--)
        {
            endCards[i].gameObject.SetActive(false);
            endCardBases[i].SetActive(false);
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        foreach (var card in endCardsPos)
        {
            card.Key.localPosition = Vector2.Lerp(card.Value.startPos, card.Value.endPos, elapsedTime / duration);
        }
    }
}