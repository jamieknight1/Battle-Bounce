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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject);
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

        pannelTitleText.text = buttonScript.scene.TrimStart('M', 'a', 'p', '_');

        pannelDescriptionText.text = buttonScript.mapDescription;
    }
}
