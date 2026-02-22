using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    ScrollRect scrollRect;
    [SerializeField] float scrollSpeed = 10f;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start() {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    void Update()
    {
        foreach (Button button in GetComponentsInChildren<Button>())
        {
            if (EventSystem.current.currentSelectedGameObject == button.gameObject)
            {
                if (button.transform.position.y - scrollRect.content.sizeDelta.y/2 < 0f)
                {
                    scrollRect.verticalNormalizedPosition -= (Time.deltaTime * scrollSpeed)/scrollRect.content.sizeDelta.y;
                }
                if (button.transform.position.y + scrollRect.content.sizeDelta.y/2 > Screen.height)
                {
                    scrollRect.verticalNormalizedPosition += (Time.deltaTime * scrollSpeed)/scrollRect.content.sizeDelta.y;
                }
            }
        }
    }
}
