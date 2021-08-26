using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Targatable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint;

    public Transform GetAimAtPoint()
    {
        return aimAtPoint;
    }
}
