using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyMe : MonoBehaviour
{
    private static DoNotDestroyMe _me;

    private void Awake()
    {
        if (_me == null)
        {
            Init();
        }

        if(_me != this)
        {
            Destroy(gameObject);
        }
    }

    private void Init()
    {
        _me = this;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
        DontDestroyOnLoad(gameObject);
    }
}
