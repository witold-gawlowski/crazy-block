using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchDownAction;
    private InputAction touchUpAction;

    private Vector2 lastPosition;
    private bool positionUpdatedThisFrame;
    private bool touchHeld;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPositionAction = playerInput.actions["MyTouchPosition"];
        touchDownAction = playerInput.actions["MyTouchDown"];
        touchUpAction = playerInput.actions["MyTouchUp"];
    }

    private void Start()
    {
        positionUpdatedThisFrame = false;
        touchHeld = false;
    }

    private void OnEnable()
    {
        touchDownAction.performed += HandleTouchDownAction;
        touchUpAction.performed += HandleTouchUpAction;
        touchPositionAction.performed += HandlePositionChanged;
    }

    private void OnDisable()
    {
        touchDownAction.performed -= HandleTouchDownAction;    
        touchUpAction.performed -= HandleTouchUpAction;
        touchPositionAction.performed -= HandlePositionChanged;
    }

    private void Update()
    {
        if(!positionUpdatedThisFrame && touchHeld)
        {
            DragManager.Instance.HandlePointerHeld(lastPosition);
        }
        positionUpdatedThisFrame = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GlobalGameManager.Instance.HandleRestart();
        }
    }

    private void HandleTouchDownAction(InputAction.CallbackContext context)
    {
        lastPosition = touchPositionAction.ReadValue<Vector2>();
        DragManager.Instance.HandlePointerDown(lastPosition);
        touchHeld = true;
        positionUpdatedThisFrame = true;
    }

    private void HandlePositionChanged(InputAction.CallbackContext context)
    {
        lastPosition = touchPositionAction.ReadValue<Vector2>();
        DragManager.Instance.HandlePointerHeld(lastPosition);
        positionUpdatedThisFrame = true;
    }

    private void HandleTouchUpAction(InputAction.CallbackContext context)
    {
        DragManager.Instance.HandlePointerUp();
        touchHeld = false;
    }

}
