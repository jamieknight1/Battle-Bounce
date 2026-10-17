using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private Image characterImage;

    void Awake()
    {
        slider = GetComponent<Slider>();
        characterImage = GetComponentInChildren<Image>();
        transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    public void SetImage(Sprite character)
    {
        characterImage.sprite = character;
        characterImage.SetNativeSize();
    }
}
