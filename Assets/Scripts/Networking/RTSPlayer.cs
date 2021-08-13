using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();
    [SerializeField] private List<Building> myBuildings;
    public List<Unit> GetMyunit()
    {
        return myUnits;
    }
    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }
    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDepawned;
        Building.ServerOnBuildingSpawn += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawned;
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Add(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Remove(building);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDepawned;
        Building.ServerOnBuildingSpawn -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawned;
    }

    private void ServerHandlerUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Add(unit);

    }
    private void ServerHandlerUnitDepawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myUnits.Remove(unit);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        if (NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDepawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDepawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandlerUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }
    private void AuthorityHandlerUnitDepawned(Unit unit)
    {
        myUnits.Remove(unit);
    }
    #endregion
}
