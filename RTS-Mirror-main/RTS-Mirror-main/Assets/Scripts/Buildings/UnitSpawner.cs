﻿using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitText;
    [SerializeField] private Image unitProgessImage;
    [SerializeField] private int maxUnitQue = 5;
    [SerializeField] private float spawnMoveRange = 10f;
    [SerializeField] private float unitSpawnDuration = 5f;
    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
             UpdateTimerDisplay();
        }
    }
    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) { return; }
        unitTimer += Time.deltaTime;

        if(unitTimer < unitSpawnDuration) { return; }

        GameObject unitInstance = Instantiate(
            unitPrefab.gameObject,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere*spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0;
    }

    [Server]
    private void ServerHandleDie()
    {
            NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if(queuedUnits == maxUnitQue) { return; }
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if(player.GetResources()< unitPrefab.GetResourceCost()) { return; }
        queuedUnits++;
        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());

    }

    #endregion

    #region Client
    private void UpdateTimerDisplay()
    {
        float newProgres = unitTimer / unitSpawnDuration;
        if(newProgres < unitProgessImage.fillAmount)
        {
            unitProgessImage.fillAmount = newProgres;

        }
        else
        {
            unitProgessImage.fillAmount = Mathf.SmoothDamp(unitProgessImage.fillAmount, newProgres, ref progressImageVelocity, 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }
    private void ClientHandleQueuedUnitsUpdated(int oldUnit, int newUnit)
    {
        remainingUnitText.text = newUnit.ToString();

    }

    #endregion
}