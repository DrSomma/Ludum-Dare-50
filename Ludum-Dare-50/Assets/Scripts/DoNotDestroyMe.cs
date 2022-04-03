using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyMe : MonoBehaviour
{
    private static bool _isInSceen;
    
    void Start()
    {
        if(_isInSceen)
            Destroy(gameObject);
        _isInSceen = true;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        DontDestroyOnLoad(gameObject);
    }
}
