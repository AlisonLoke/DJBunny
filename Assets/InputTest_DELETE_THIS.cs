using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest_DELETE_THIS : MonoBehaviour
{
    private InputAction pickupDropAction;
    private InputAction rotateAction;
    private InputAction moveAction;

    private void Awake()
    {
        pickupDropAction = InputSystem.actions.FindAction("Gameplay/Pickup/Drop");
        rotateAction = InputSystem.actions.FindAction("Gameplay/Rotate");
        moveAction = InputSystem.actions.FindAction("Gameplay/MoveCursor");

        pickupDropAction.performed += OnPickupOrDrop;
        rotateAction.performed += OnRotate;
        moveAction.performed += OnMoveCursor;
    }

    private void OnDisable()
    {
        pickupDropAction.performed -= OnPickupOrDrop;
        rotateAction.performed -= OnRotate;
        moveAction.performed -= OnMoveCursor;
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
    }

    private void OnPickupOrDrop(InputAction.CallbackContext context)
    {
    }

    private void OnMoveCursor(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        Debug.Log($"Move cursor action detected, value: {inputValue}");
    }
}
