using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnitMovement unitMovement;
    [SerializeField] private Targeter targeter;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeseletected;
    [SerializeField] private Health health;


    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;

    #region server
    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);

    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject); 
    }


    #endregion
    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    public Targeter GetTargeter()
    {
        return targeter;
    }
    #region client

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }


    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        onSelected?.Invoke();
    }
    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeseletected?.Invoke();
    }
    #endregion
}
