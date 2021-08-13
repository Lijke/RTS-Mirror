using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;
using System;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform unitSpawnPoint;


    #region server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    
    private void HandleServerOnDie()
    {
      //  health.ServerOnDie -= ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    #endregion
    #region Client
    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }
        if (!hasAuthority) { return; }
        CmdSpawnUnit();
    }

    #endregion
}
