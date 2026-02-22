using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCard : MonoBehaviour
{
    public GameObject cursor;
    [SerializeField] private Image characterImage;

    public bool isCpu = false;

    void Update()
    {
        if (cursor != null)
        {
            SetCharacterImage();
        }
    }

    private void SetCharacterImage()
    {
        Collider2D target = Physics2D.OverlapPoint(cursor.transform.position, LayerMask.GetMask("PlayerObjects"), -1000f, 1000f);
        if (target != null)
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
}
