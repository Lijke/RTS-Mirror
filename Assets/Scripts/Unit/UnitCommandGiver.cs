using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler;
    [SerializeField] private LayerMask layerMask;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandlerer.ClientOnGameOver += ClientHandleGameOver;
    }
    private void OnDestroy()
    {
        GameOverHandlerer.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if(hit.collider.TryGetComponent<Targatable>(out Targatable target))
        {
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }
            TryTarget(target);
            return;
        }

        TryMove(hit.point);
    }

    private void TryTarget(Targatable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnit)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnit)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }
}
