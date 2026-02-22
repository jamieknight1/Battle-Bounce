using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulesetManager : MonoBehaviour
{
    [HideInInspector] public int lives;
    [HideInInspector] public int time;
    [HideInInspector] public int gamemode;
    [HideInInspector] public string rulesetName;

    [SerializeField] TMP_Dropdown timeField;
    [SerializeField] TMP_Dropdown gamemodeField;
    [SerializeField] TMP_InputField lifeField;
    [SerializeField] TMP_InputField rulesetNameField;

    [SerializeField] Button createRulesetButton;

    [SerializeField] GameObject rulesetButtonPf;

    [Space(10)]

    [SerializeField] GameObject createRulesetPanel;
    [SerializeField] RectTransform contentPanel;

    TMP_Dropdown.OptionData infinateOption;
    Navigation noNav = new Navigation();
    Navigation yesNav;

    public bool editMode = false;

    public RulesetButton rulesetButton;
    GameObject rulesetInfo;

    public void OnSave()
    {
        if (rulesetNameField.text != null && lifeField.text != null)
        {
            createRulesetPanel.SetActive(false);

            lives = int.Parse(lifeField.text);
            time = timeField.value;
            gamemode = gamemodeField.value;
            rulesetName = rulesetNameField.text;

            if (!editMode)
            {
                GameObject rulesetButton = Instantiate(rulesetButtonPf);
                RulesetButton rulesetButtonScript = rulesetButton.GetComponent<RulesetButton>();
                TMP_Text rulesetButtonText = rulesetButton.GetComponentInChildren<TMP_Text>();
                RectTransform rulesetButtonRecttransform = rulesetButton.GetComponent<RectTransform>();

                rulesetButtonScript.lives = lives;
                rulesetButtonScript.time = time;
                rulesetButtonScript.gamemode = gamemode;
                rulesetButtonText.text = rulesetName;

                rulesetButtonRecttransform.SetParent(contentPanel);

                createRulesetButton.Select();
            }

            if (editMode)
            {
                rulesetButton.lives = lives;
                rulesetButton.time = time;
                rulesetButton.gamemode = gamemode;
                rulesetButton.rulesetName = rulesetName;
                rulesetButton.rulesetButtonText.GetComponent<TMP_Text>().text = rulesetName;
                rulesetButton.GetComponent<Button>().Select();
                editMode = false;
            }
            //RectTransform[] contentChildren;
            //contentChildren = contentPanel.GetComponentsInChildren<RectTransform>();

            //rulesetButtonRecttransform.anchoredPosition = new Vector2(0, 100 - 50 * (contentChildren.Length - 1)/2);
            //Debug.Log(contentChildren.Length);

            rulesetInfo.SetActive(true);
        }
    }

    public void CreateRulesetButton()
    {
        rulesetNameField.Select();

        createRulesetPanel.SetActive(true);
    }

    public void OnRulesetChange()
    {
        if (gamemodeField.value == 1)
        {
            timeField.options.RemoveAt(6);
            timeField.value = 0;

            lifeField.navigation = noNav;
        }

        if (gamemodeField.value == 0)
        {
            timeField.options.Add(infinateOption);

            lifeField.navigation = yesNav;
        }
    }

    void Start()
    {
        rulesetInfo = GameObject.Find("Ruleset Info");
        yesNav = lifeField.navigation;
        noNav.mode = Navigation.Mode.None;
        infinateOption = timeField.options[6];
    }
}
