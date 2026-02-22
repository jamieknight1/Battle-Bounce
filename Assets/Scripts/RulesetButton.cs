using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RulesetButton : MonoBehaviour
{
    public int lives;
    public int time;
    public int gamemode;
    public string rulesetName;
    private bool childSelected = false;

    [SerializeField] GameSetup gameSetup;
    [SerializeField] public Transform rulesetButtonText;

    [SerializeField] RulesetManager rulesetManager;

    private GameObject rulesetInfo;

    void Awake()
    {
        rulesetInfo = GameObject.Find("Ruleset Info");
    }

    private void Update()
    {
        foreach (Button child in GetComponentsInChildren<Button>())
        {
            if (EventSystem.current.currentSelectedGameObject == child.gameObject)
            {
                childSelected = true;
                break;
            }

            else if (EventSystem.current.currentSelectedGameObject != child.gameObject)
            {
                childSelected = false;
            }
        }

        if (!childSelected)
        {
            foreach (Button child in GetComponentsInChildren<Button>())
            {
                child.gameObject.SetActive(false);
                rulesetButtonText.gameObject.SetActive(true);
                gameObject.SetActive(true);
            }
        }

        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            rulesetInfo.SetActive(true);
            UpdateRulesetInfo();
        }

        if (EventSystem.current.currentSelectedGameObject == GameObject.Find("Create Ruleset Button")) { rulesetInfo.SetActive(false); }
    }

    public void RulesetSelected()
    {
        foreach (RectTransform child in transform)
        {
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    child.gameObject.SetActive(true);
                    rulesetButtonText.gameObject.SetActive(true);

                    GetComponentsInChildren<Button>()[1].Select();
                }

                else if (child.gameObject.activeInHierarchy) { child.gameObject.SetActive(false); rulesetButtonText.gameObject.SetActive(true); }
            }
        }
    }

    public void OnDelete()
    {
        GameObject.Find("Create Ruleset Button").GetComponent<Button>().Select();
        Destroy(gameObject);
    }

    public void OnEdit()
    {
        Debug.Log(transform.name);
        rulesetManager = FindObjectOfType<RulesetManager>();
        rulesetManager.rulesetButton = this;
        rulesetManager.editMode = true;
        rulesetManager.CreateRulesetButton();

        rulesetInfo.SetActive(false);
    }

    public void OnStart()
    {
        gameSetup.rulesetTime = time;
        gameSetup.rulesetGamemode = gamemode;
        gameSetup.playerLives = lives;

        SceneManager.LoadScene("MapSelection");
    }

    void UpdateRulesetInfo()
    {
        TMP_Text[] rulesetInfoText = rulesetInfo.GetComponentsInChildren<TMP_Text>();
        rulesetInfoText[0].text = rulesetName;

        if (time == 6) { rulesetInfoText[1].text = "Time: Infinate"; }
        else if (time == 0) { rulesetInfoText[1].text = "Time: 1 Minute"; }
        else { rulesetInfoText[1].text = "Time: " + gameSetup.seconds[time] / 60 + " Minutes"; }

        rulesetInfoText[2].text = "Lives: " + lives.ToString();
        rulesetInfoText[3].text = "Gamemode: " + gameSetup.rulesets[gamemode];
    }
}
