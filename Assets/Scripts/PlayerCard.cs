using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public GameObject cursor;
    [SerializeField] private Image characterImage;
    HandCursor cursorScript;

    public bool isCpu = false;

    void Update()
    {
        if (cursor == null || cursorScript.holdingState == HoldingState.HoldingNothing) return;
        if (cursor != null && cursorScript.holdingState == HoldingState.HoldingPlayer || (isCpu && cursorScript.holdingState == HoldingState.HoldingCPU))
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

    public void RemoveCursorScript()
    {
        cursorScript = null;
    }
}
