using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public GameObject cursor {get; private set;}
    [SerializeField] private Image characterImage;
    [SerializeField] Sprite cpuCardSprite;
    [SerializeField] Sprite playerCardSprite;
    HandCursor cursorScript;
    public PlayerData player {get; private set;}

    public bool isCpu = false;

    void Update()
    {
        if (cursor == null || cursorScript.holdingState == HoldingState.HoldingNothing) return;
        if (cursor != null && (cursorScript.holdingState == HoldingState.HoldingPlayer || (isCpu && cursorScript.holdingState == HoldingState.HoldingCPU)))
        {
            SetCharacterImage();
        }

    }

    private void SetCharacterImage()
    {
        Collider2D target = Physics2D.OverlapPoint(cursor.transform.position, LayerMask.GetMask("PlayerObjects"), -1000f, 1000f);
        if (target != null && !target.CompareTag("PlayerCard"))
        {
            characterImage.gameObject.SetActive(true);
            SpriteRenderer targetSprite = target.GetComponent<SpriteRenderer>();
            characterImage.sprite = targetSprite.sprite;
            characterImage.SetNativeSize();
        }

        if (target == null)
        {
            characterImage.gameObject.SetActive(false);
        }
    }

    public void InitializeHandCursorScript()
    {
        cursorScript = cursor.GetComponent<HandCursor>();
    }

    public void SetCursor(GameObject handCursor)
    {
        cursor = handCursor;
    }

    public void RemoveCursorScript()
    {
        cursorScript = null;
    }

    public void RemoveCursor()
    {
        cursor = null;
    }

    public void SwitchSprite()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (isCpu) spriteRenderer.sprite = cpuCardSprite;
        else spriteRenderer.sprite = playerCardSprite;
    }

    public void AddPlayer(PlayerData playerToAdd)
    {
        player = playerToAdd;
    }

    public void RemovePlayer()
    {
        isCpu = false;
        SwitchSprite();
        player = null;
        RemoveCursorScript();
        cursor = null;
        characterImage.gameObject.SetActive(false);
    }
}
