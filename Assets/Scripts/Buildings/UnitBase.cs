using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health;
    public static event Action<UnitBase> ServerOnBaseSpawn;
    public static event Action<UnitBase> ServerOnBaseDespawn;

    public static event Action<int> ServerOnPlayerDie;

    #region server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawn?.Invoke(this);
    }
    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        ServerOnBaseDespawn?.Invoke(this);
    }


    [Server]
    private void ServerHandleDie()
    {

        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);

        NetworkServer.Destroy(gameObject);
    }

    #endregion
    #region Client
    #endregion
}
