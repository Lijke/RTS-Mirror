using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Targeter : NetworkBehaviour
{
   private Targatable target;
    public Targatable GetTarget()
    {
        return target;
    }
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
        ClearTarget();
    }

    [Command]
   public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targatable>(out Targatable target)) { return; }

        this.target = target;

    }
    [Server]
    public void ClearTarget()
    {
        target = null;
    }

}
