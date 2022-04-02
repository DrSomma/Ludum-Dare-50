using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Vector3 target;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Teleport to: " + target);
        col.transform.position = target;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(target, 0.2f);
    }
}
