using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapPannel : MonoBehaviour
{
    [SerializeField] Image pannelImage;
    [SerializeField] TMP_Text pannelTitleText;
    [SerializeField] TMP_Text pannelDescriptionText;
    
    void Update()
    {
        UpdateImage();
        UpdateText();
    }

    void UpdateImage()
    {
        pannelImage.sprite = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
    }

    void UpdateText()
    {
        CursorButtonSelection buttonScript = EventSystem.current.currentSelectedGameObject.GetComponent<CursorButtonSelection>();

        pannelTitleText.text = buttonScript.scene.Substring(4);

        pannelDescriptionText.text = buttonScript.mapDescription;
    }
}
