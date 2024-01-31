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
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPositionAction = playerInput.actions["MyTouchPosition"];
        touchDownAction = playerInput.actions["MyTouchDown"];
        touchUpAction = playerInput.actions["MyTouchUp"];
    }

    private void OnEnable()
    {
        touchDownAction.performed += HandleTouchDownAction;
        touchUpAction.performed += HandleTouchUpAction;
        touchPositionAction.performed += HandleTOuchHoldAction;
    }

    private void OnDisable()
    {
        touchDownAction.performed -= HandleTouchDownAction;    
        touchUpAction.performed -= HandleTouchUpAction;
        touchPositionAction.performed -= HandleTOuchHoldAction;
    }

    private void Update()
    {
        //DragManager.Instance.HandlePointerHeld(touchPosition);
    }

    private void HandleTouchDownAction(InputAction.CallbackContext context)
    {
        var touchPosition = touchPositionAction.ReadValue<Vector2>();
        DragManager.Instance.HandlePointerDown(touchPosition);
    }

    private void HandleTOuchHoldAction(InputAction.CallbackContext context)
    {
        var touchPosition = touchPositionAction.ReadValue<Vector2>();
        DragManager.Instance.HandlePointerHeld(touchPosition);
    }

    private void HandleTouchUpAction(InputAction.CallbackContext context)
    {
        DragManager.Instance.HandlePointerUp();
    }

}
