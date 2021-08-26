using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class CameraControler : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits;
    [SerializeField] private Vector2 screenZLimits;
    private Controls controls;
    private Vector2 previusInput;
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        controls.Enable();
    }
    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();

    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        if (previusInput == Vector2.zero)
        {
            Vector3 cursosMovement = Vector3.zero;
            Vector2 cursosPosition = Mouse.current.position.ReadValue();

            if (cursosPosition.y >= Screen.height - screenBorderThickness)
            {
                cursosMovement.z += 1;
            }
            else if (cursosPosition.y <= screenBorderThickness)
            {
                cursosMovement.z -= 1;
            }
            if (cursosPosition.x >= Screen.width - screenBorderThickness)
            {
                cursosMovement.x += 1;
            }
            else if (cursosPosition.x <= screenBorderThickness)
            {
                cursosMovement.x -= 1;
            }
            pos += cursosMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previusInput.x, 0, previusInput.y) * speed * Time.deltaTime;

        }
        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);
        playerCameraTransform.position = pos;
    }




    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previusInput = ctx.ReadValue<Vector2>();

    }
}
