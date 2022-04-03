using DG.Tweening;
using UnityEngine;

namespace Platforms
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private Vector2 endPosRelativ;
        
        private Vector3 EndPos => new Vector3(x: transform.position.x + endPosRelativ.x, y: transform.position.y + endPosRelativ.y);

        private void Start()
        {
            transform.DOMove(endValue: EndPos, duration: speed).SetEase(Ease.InOutSine).SetSpeedBased().SetLoops(loops: -1,loopType: LoopType.Yoyo);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            col.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            col.transform.SetParent(null);
        }

        #region Debug
        private SpriteRenderer _renderer; 
        private Vector3? _endPos; 
        private void OnDrawGizmos()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            if (!_endPos.HasValue) _endPos = EndPos;
            Vector3 position = transform.position;
            Gizmos.DrawWireCube(center: _endPos.Value,_renderer.size);
        }
        #endregion
       
    }
}
