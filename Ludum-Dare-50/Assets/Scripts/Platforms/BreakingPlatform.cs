using DG.Tweening;
using UnityEngine;

namespace Platforms
{
    public class BreakingPlatform : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
    
        [SerializeField]
        private float strength = 20f;
        [SerializeField]
        private int loopCount = 3;
    
        private bool _isBreaking;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }

            if (!IsPlayerAbove(col))
            {
                return;
            }
            
            Break();
        }

        private bool IsPlayerAbove(Collision2D col)
        {
            return col.transform.position.y < transform.position.y;
        }

        //Just for debug!
        // private void Update()
        // {
        //     if(Input.GetKeyDown(KeyCode.G))
        //         Break();
        //     if (Input.GetKeyDown(KeyCode.H))
        //     {
        //         _isBreaking = false;
        //         Debug.Log("Kill: " + transform.DOKill(true));
        //         transform.rotation = Quaternion.identity;
        //     }
        // }

        private void Break()
        {
            if (_isBreaking)
            {
                return;
            }

            _isBreaking = true;
        
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DORotate(endValue: Vector3.forward * strength, duration: speed, mode: RotateMode.FastBeyond360));
            sq.Append(transform.DORotate(endValue: Vector3.back * strength, duration: speed, mode: RotateMode.FastBeyond360));
            sq.SetLoops(loops: loopCount, loopType: LoopType.Yoyo);
            sq.OnComplete(DestroyMe);
        }

        private void DestroyMe()
        {
            SoundManager.Instance.PlaySound(SoundManager.Sounds.Break);
            
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Vector3 position = transform.position;
            Vector3 endPos = new Vector3(x: position.x, y: position.y - 5);
            transform.DOMove(endValue: endPos, duration: 0.3f).SetEase(Ease.InExpo);
            sprite.DOFade(endValue: 0, duration: 0.3f);
            
            Destroy(obj: gameObject,t: 2f);
        }
    }
}
