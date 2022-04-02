using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exit!");
            var playerMovement = col.GetComponent<PlayerMovement>();
            playerMovement.OnFoundExit();
        }
    }
}
