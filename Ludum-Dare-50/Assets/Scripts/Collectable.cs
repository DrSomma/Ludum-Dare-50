using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public static event Action OnCollect;
    
    private async void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
        {
            return;
        }
        OnCollect?.Invoke();
        await DestroyMe();
    }

    private async Task DestroyMe()
    {
        await transform.DOShakeScale(0.3f).SetEase(Ease.OutElastic).AsyncWaitForCompletion();
        Destroy(gameObject);
    }
}
