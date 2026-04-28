using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandCursor : MonoBehaviour
{
    private PlayerManager playerManager;
    private InputDevice inputDevice;
    private PlayerInput playerInput;
    private CharacterSelectSession characterSelectSession;
    public PlayerData PlayerData {get; private set;}

    private Vector2 movementDirection;
    [SerializeField] private float movementSpeed;
    private float screenEdgeThreshold = 0.02f;

    public event Action<HandCursor> SelectPressed;
    public event Action<HandCursor> StartPressed;
    public event Action<HandCursor> CancelPressed;

    [SerializeField] private GameObject cursorIcon;
    public CpuCursor cpuCursorIcon {get; private set;}

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite closedHand;
    [SerializeField] private Sprite openedHand;

    public HoldingState holdingState {get; private set;}
    public PlayerCard CpuCard {get; private set;}

    void OnEnable()
    {
        var uiMap = playerInput.actions.FindActionMap("UI");

        uiMap.FindAction("Navigate").performed += OnNavigate;
        uiMap.FindAction("Navigate").canceled += OnNavigate;

        uiMap.FindAction("Select").started += OnSelect;
        uiMap.FindAction("Start").started += OnStart;
        uiMap.FindAction("Cancel").started += OnCancel;

        characterSelectSession.RegisterCursor(this);
    }

    void OnDisable()
    {
        var uiMap = playerInput.actions.FindActionMap("UI");

        uiMap.FindAction("Navigate").performed -= OnNavigate;
        uiMap.FindAction("Navigate").canceled -= OnNavigate;

        uiMap.FindAction("Select").started -= OnSelect;
        uiMap.FindAction("Start").started -= OnStart;
        uiMap.FindAction("Cancel").started -= OnCancel;

        characterSelectSession.UnregisterCursor(this);
    }

    void OnDestroy()
    {
        playerInput.actions["Select"].started -= OnSelect;
        playerInput.actions["Start"].started -= OnStart;
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        characterSelectSession = FindObjectOfType<CharacterSelectSession>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveCursor();
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) movementDirection = context.ReadValue<Vector2>();

        if (context.phase == InputActionPhase.Canceled) movementDirection = Vector2.zero;
    }

    private void MoveCursor()
    {
        var viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if ((viewportPosition.x < screenEdgeThreshold && movementDirection.x < 0) ||
            (viewportPosition.x > 1 - screenEdgeThreshold && movementDirection.x > 0) ||
            (viewportPosition.y < screenEdgeThreshold && movementDirection.y < 0) ||
            (viewportPosition.y > 1 - screenEdgeThreshold && movementDirection.y > 0))
            return;

        transform.Translate(movementDirection * movementSpeed * Time.deltaTime);
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {   
            SelectPressed?.Invoke(this);
        }
    }

    public void OnStart(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            StartPressed?.Invoke(this);
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            CancelPressed?.Invoke(this);
        }
    }

    public void InitializePlayerData(PlayerData playerData)
    {
        PlayerData = playerData;
    }

    public void LockCursorIconPosition()
    {
        cursorIcon.transform.SetParent(null);
        OpenHand();
    }

    public void LockCpuCursorIconPosition()
    {
        cpuCursorIcon.transform.SetParent(null);
        OpenHand();
    }

    public void UnlockCursorIconPosition()
    {
        cursorIcon.transform.SetParent(transform);
        cursorIcon.transform.localPosition = Vector2.zero;
        CloseHand();
    }

    public void SetCursorIconImage(Sprite sprite)
    {
        SpriteRenderer cursorIconSprite = cursorIcon.GetComponent<SpriteRenderer>();
        cursorIconSprite.sprite = sprite;
    }

    public void OpenHand()
    {
        spriteRenderer.sprite = openedHand;
    }

    public void CloseHand()
    {
        spriteRenderer.sprite = closedHand;
    }

    public void SetCpuCursor(CpuCursor cursor)
    {
        cpuCursorIcon = cursor;
    }

    public void RemoveCpuCursor()
    {
        cpuCursorIcon = null;
    }

    public void SetCpuCard(PlayerCard card)
    {
        CpuCard = card;
    }

    public void RemoveCpuCard()
    {
        CpuCard = null;
    }

    public void ChangeHoldingState(HoldingState state)
    {
        holdingState = state;
    }
}

public enum HoldingState
{
    HoldingPlayer,
    HoldingCPU,
    HoldingNothing
}
