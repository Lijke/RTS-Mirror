using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter;
    [SerializeField] private float chaseRange = 10f;
    #region Server

    public override void OnStartServer()
    {
        GameOverHandlerer.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandlerer.ServerOnGameOver -= ServerHandleGameOver;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    [ServerCallback]
    private void Update()
    {
        Targatable target = targeter.GetTarget();
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude>chaseRange*chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }


            return;
        }
        if (!agent.hasPath) { return; }
        if(agent.remainingDistance> agent.stoppingDistance) { return; }
        agent.ResetPath();
    }


    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        agent.SetDestination(hit.position);
    }

    #endregion

}
