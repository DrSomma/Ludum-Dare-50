using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private new ParticleSystem particleSystem;
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
        particleSystem.Play();
        await transform.DOShakeScale(0.3f).SetEase(Ease.OutElastic).AsyncWaitForCompletion();
        Destroy(gameObject);
    }
}
