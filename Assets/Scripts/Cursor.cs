using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    /*PlayerInput playerInput;
    private InputActionAsset inputAsset;
    private InputActionMap cursor;
    public PlayerInputManager inputManager;
    private Vector2 cursorMovement;
    [SerializeField] private float sensitivity = 0.05f;
    [SerializeField] private float deadZone = 1f;
    public Transform canvas;
    public Button selectedButton;
    public List<Button> buttonList = new List<Button>();*/

    public int numberOfPlayers = 4;

    void Awake()
    {
        /*canvas = FindObjectOfType<Canvas>().transform;
        inputManager = FindObjectOfType<PlayerInputManager>();
        inputAsset = this.GetComponent<PlayerInput>().actions;
        cursor = inputAsset.FindActionMap("Cursor");
        transform.SetParent(canvas);*/
        for (int players = 0; players < numberOfPlayers; players++)
        {
            Instantiate(gameObject);
        }
    }

    /*void OnEnable()
    {
        cursor.Enable();
    }

    void OnDisable()
    {
        cursor.Disable();
    }*/

    void Update()
    {
        /*cursorMovement = cursor.FindAction("Point").ReadValue<Vector2>();
        if (cursorMovement.magnitude > deadZone)
        {
            transform.Translate(cursorMovement * sensitivity);
        }

        foreach (Button button in collection)
        {
             (int i = 0; i < length; i++)
            {
                if (transform.position.x >=)
            }
        }*/
    }
}
